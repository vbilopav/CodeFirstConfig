using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public static partial class Configurator
    {
        private static FileSystemWatcher _watcher = null;
        private static Stopwatch _stopwatch = null;
        private static bool _configured = false;
        private static TimedConsumer _consumer;
        private static IEnumerable<Type> _types;
        private static Func<IEnumerable<Type>> _getConfigTypesFunc = () => AppDomain.CurrentDomain.GetAssemblies()
            .Where(c => !c.GlobalAssemblyCache && !c.CodeBase.Contains("Temporary") && c.GetName().GetPublicKeyToken().Length == 0)
            .SelectMany(c => c.GetTypes())
            .Where(c => c != typeof(ConfigManager<>))
            .Where(c => c.IsSubclassOfRawGeneric(typeof(ConfigManager<>)));

        private static List<Exception> _exceptions = new List<Exception>();

        internal static void AddExceptions(Exception exc)
        {
            _exceptions.Add(exc);
        }

        internal static object ConfigLock = new object();

        private static void InitializeWatcher()
        {
            const ushort reconfigureDelayMs = 250;
            const ushort eventWindowMs = 1000;
            _watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                IncludeSubdirectories = false
            };
            _stopwatch = new Stopwatch();
            Func<Task> handler = async () =>
            {
                if (_stopwatch.IsRunning && _stopwatch.ElapsedMilliseconds <= eventWindowMs) return;
                _stopwatch.Reset();
                _stopwatch.Start();
                _watcher.EnableRaisingEvents = false;
                await Task.Delay(reconfigureDelayMs);
                await ReconfigureAsync();
                _watcher.EnableRaisingEvents = true;
                _stopwatch.Reset();
            };
            _watcher.InternalBufferSize = _watcher.InternalBufferSize*4;
            _watcher.Error += (sender, e) => {
                if (ConfigSettings.Instance.OnError != null)
                    ConfigSettings.Instance.OnError(new ConfigErrorEventArgs(e.GetException()));
            };
            _watcher.Changed += (sender, e) => Task.Run(handler);         
            _watcher.Renamed += (sender, e) => Task.Run(handler);
            _watcher.EnableRaisingEvents = false;
            AppFinalizator.CleanupQueue.Enqueue(_watcher);
        }

        private static object GetConfigValue(Type t)
        {
            var p =
                t.GetProperty("Config", BindingFlags.Public | BindingFlags.Static) ??
                t.GetProperty("Config", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (p == null || t.BaseType == null) return null;
            return p.GetValue(null);
        }

        private static object InvokeReconfigure(Type t)
        {
            var m =
                t.GetMethod("Reconfigure", BindingFlags.Public | BindingFlags.Static) ??
                t.GetMethod("Reconfigure", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return m?.Invoke(null, null);
        }

        private static void CheckIsConfigured()
        {
            if (_configured)
                throw new CodeFirstConfigException("Configure method should be called only once per AppDomain!");
        }

        private static void CheckIsNotConfigured()
        {
            if (!_configured)
                throw new CodeFirstConfigException("Configure must be called before any Reconfigure calls!");
        }

        private static IDictionary<string, object> ConfigureInternal(Action<Type> action,
            DbConfigOptions databaseOptions = null,
            bool waitBeforeFirstWrite = false,
            bool refreshAppSettings = false,
            ConfigSettings settings = null)
        {
            lock (ConfigLock)
            {
                _exceptions = new List<Exception>();
                Type appType = typeof(AppConfigManager);                
                if (_types == null) _types = _getConfigTypesFunc();

                try
                {
                    if (settings != null)
                        ConfigSettings.Instance = settings;
                    //ConfigSettings.Instance = settings ?? new ConfigSettings();
                    ConfigValues.DatabaseOptions = databaseOptions;
                    ConfigValues.Reconfigure(refreshAppSettings);
                    action(appType);
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }

                foreach (Type t in _types)
                {
                    try
                    {
                        if (t == appType) continue;
                        action(t);
                    }
                    catch (Exception e)
                    {
                        _exceptions.Add(e);
                    }
                }
                ConfigObjects.SetTimeStamp();
                try
                {
                    if (ConfigValues.DatabaseOptions != null)
                    {
                        #pragma warning disable 4014
                        ConfigValues.DatabaseOptions.InsertInstanceAsync(ConfigObjects.Current);
                        #pragma warning restore 4014
                    }
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }
                try
                {
                    var watching = false;
                    if (_watcher != null)
                    {
                        watching = _watcher.EnableRaisingEvents;
                        if (watching) _watcher.EnableRaisingEvents = false;
                    }
                    if (ConfigSettings.Instance.SaveConfigFile)
                    {
                        SaveConfigFile(waitBeforeFirstWrite);
                    }
                    if (ConfigSettings.Instance.EnableFileWatcher)
                    {
                        if (_watcher == null) InitializeWatcher();
                        if (string.IsNullOrEmpty(_watcher.Path))
                        {
                            _watcher.Path = Path.GetDirectoryName(ConfigSettings.Instance.ConfigFileName);
                            _watcher.Filter = Path.GetFileName(ConfigSettings.Instance.ConfigFileName);
                            _watcher.EnableRaisingEvents = true;
                        }
                    }
                    if (watching && _watcher != null && !_watcher.EnableRaisingEvents) _watcher.EnableRaisingEvents = true;
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }
                try
                {
                    if (ConfigSettings.Instance.EnableTimer && _consumer == null)
                    {
                        _consumer = new TimedConsumer(1, (int)ConfigSettings.Instance.TimerMinutes * 1000 * 60);
                        AppFinalizator.CleanupQueue.Enqueue(_consumer);
                        _consumer.Start(() => Reconfigure());
                    }
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }

                if (!_configured) _configured = true;
                if (!_exceptions.Any()) return ConfigObjects.Current;
                var exception = new AggregateException(_exceptions.ToArray());
                if (ConfigSettings.Instance.ThrowOnConfigureException)
                    throw exception;
                if (ConfigSettings.Instance.OnError != null)
                    ConfigSettings.Instance.OnError(new ConfigErrorEventArgs(exception));
                return ConfigObjects.Current;
            }
        }

        private static void SaveConfigFile(bool waitBeforeFirstWrite)
        {
            if (waitBeforeFirstWrite) Task.Delay(ConfigSettings.DelayReadWriteLockedFileAfterExceptionMs).Wait();
            for (int retry = 0; retry < ConfigSettings.MaxRetryBeforeException; retry++)
            {
                try
                {
                    ConfigObjects.ToFile(ConfigSettings.Instance.ConfigFileName, _exceptions);
                    break;
                }
                catch (IOException)
                {
                    if (retry < ConfigSettings.MaxRetryBeforeException - 1)
                    {
                        Task.Delay(ConfigSettings.DelayReadWriteLockedFileAfterExceptionMs).Wait();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}