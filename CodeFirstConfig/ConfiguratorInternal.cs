using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public static partial class Configurator
    {
        static Configurator()
        {
            Watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            Watcher.Changed += async (sender, e) =>
            {
                Watcher.EnableRaisingEvents = false;
                await ReconfigureAsync();
                Watcher.EnableRaisingEvents = true;
            };
            Watcher.EnableRaisingEvents = false;
            AppFinalizator.CleanupQueue.Enqueue(Watcher);
        }

        private static readonly FileSystemWatcher Watcher;
        private static bool _configured = false;
        private static TimedConsumer _consumer;
        private static IEnumerable<Type> _types;
        private static Func<IEnumerable<Type>> _getConfigTypesFunc = () => AppDomain.CurrentDomain.GetAssemblies()
            .Where(c => !c.GlobalAssemblyCache && !c.CodeBase.Contains("Temporary") && c.GetName().GetPublicKeyToken().Length == 0)
            .SelectMany(c => c.GetTypes())
            .Where(c => c != typeof(ConfigManager<>))
            .Where(c => c.IsSubclassOfRawGeneric(typeof(ConfigManager<>)));

        private static List<Exception> _exceptions;

        internal static object ConfigLock = new object();

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
                    ConfigSettings.Instance = settings ?? new ConfigSettings();
                    action(appType);                                  
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }

                try
                {
                    ConfigValues.DatabaseOptions = databaseOptions;
                    ConfigValues.Reconfigure(refreshAppSettings);
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
                        var task = ConfigValues.DatabaseOptions.InsertInstanceAsync(ConfigObjects.Current);
                    }
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }
                try
                {
                    if (ConfigSettings.Instance.SaveConfigFile)
                    {
                        bool watching = Watcher.EnableRaisingEvents;
                        if (watching) Watcher.EnableRaisingEvents = false;

                        const int delay = 250;
                        if (waitBeforeFirstWrite) Task.Delay(delay).Wait();
                        for (int retry = 0; retry < ConfigSettings.Instance.MaxFileWriteRetry; retry++)
                        {
                            try
                            {
                                ConfigObjects.ToFile(ConfigSettings.Instance.SaveConfigFileName, _exceptions);
                                break;
                            }
                            catch (IOException)
                            {
                                if (retry < ConfigSettings.Instance.MaxFileWriteRetry - 1)
                                {
                                    Task.Delay(delay).Wait();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(Watcher.Path) && ConfigSettings.Instance.EnableFileWatcher)
                        {
                            Watcher.Path = Path.GetDirectoryName(ConfigSettings.Instance.SaveConfigFileName);
                            Watcher.Filter = Path.GetFileName(ConfigSettings.Instance.SaveConfigFileName);
                            Watcher.EnableRaisingEvents = true;
                        }

                        if (watching && !Watcher.EnableRaisingEvents) Watcher.EnableRaisingEvents = true;
                    }
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
                if (ConfigSettings.Instance.ThrowOnConfigureException && _exceptions.Any())
                    throw new AggregateException(new AggregateException(_exceptions.ToArray()));
                return ConfigObjects.Current;
            }
        }
    }
}