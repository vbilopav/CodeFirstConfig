using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public static partial class Configurator
    {
        private static Action<ConfigAfterSetEventArgs> AfterSetEvent { get; set; }
        private static Action<ConfigBeforeSetEventArgs> BeforeSetEvent { get; set; }

        public static Action<ConfigAfterSetEventArgs> OnAfterSet { get { return AfterSetEvent; } set { lock (ConfigLock) { AfterSetEvent = value; } } }
        public static Action<ConfigBeforeSetEventArgs> OnBeforeSet { get { return BeforeSetEvent; } set { lock (ConfigLock) { BeforeSetEvent = value; } } }

        public static AggregateException Exception { get { lock (ConfigLock) return new AggregateException(_exceptions.ToArray()); } }
        public static IDictionary<string, object> Current => ConfigObjects.Current;

        public static void SetConfigTypesFunc(Func<IEnumerable<Type>> func)
        {
            lock (ConfigLock)
            {
                _types = null;
                _getConfigTypesFunc = func;
            }
        }

        public static string SerializeCurrent()
        {
            lock (ConfigLock) using (var sw = new StringWriter())
            {
                ConfigObjects.ToWriter(sw, exceptions: _exceptions);
                return sw.ToString();
            }
        }

        public static void SerializeCurrent(TextWriter writer)
        {
            lock (ConfigLock) ConfigObjects.ToWriter(writer, exceptions: _exceptions);                               
        }

        public static string SerializeCurrent(ConfigFormat format)
        {
            lock (ConfigLock) using (var sw = new StringWriter())
            {
                ConfigObjects.ToWriter(sw, format, exceptions: _exceptions);
                return sw.ToString();
            }
        }

        public static void SerializeCurrent(TextWriter writer, ConfigFormat format)
        {
            lock (ConfigLock) ConfigObjects.ToWriter(writer, format, exceptions: _exceptions);
        }

        public static IDictionary<string, object> Configure(ConfigSettings settings = null)
        {
            CheckIsConfigured();
            return ConfigureInternal(t => GetConfigValue(t), 
                waitBeforeFirstWrite: false, 
                refreshAppSettings: false,
                settings: settings);
        }

        public static IDictionary<string, object> Configure(DbConfigOptions databaseOptions, ConfigSettings settings = null)
        {
            CheckIsConfigured();
            return ConfigureInternal(t => GetConfigValue(t), 
                databaseOptions: databaseOptions, 
                waitBeforeFirstWrite: false, 
                refreshAppSettings: false,
                settings: settings);
        }

        public static async Task<IDictionary<string, object>> ConfigureAsync(ConfigSettings settings = null)
        {
            CheckIsConfigured();
            return await Task.Run(() => ConfigureInternal(t => GetConfigValue(t), 
                waitBeforeFirstWrite: true, 
                refreshAppSettings: false,
                settings: settings));
        }

        public static async Task<IDictionary<string, object>> ConfigureAsync(DbConfigOptions databaseOptions, ConfigSettings settings = null)
        {            
            CheckIsConfigured();
            return await Task.Run(() => ConfigureInternal(t => GetConfigValue(t),
                databaseOptions: databaseOptions,
                waitBeforeFirstWrite: true,
                refreshAppSettings: false,
                settings: settings));
        }

        public static void SetDatabaseOptions(DbConfigOptions databaseOptions)
        {
            lock (ConfigLock)
            {
                ConfigValues.DatabaseOptions = databaseOptions;
                ConfigValues.Reconfigure(false);
            }
        }

        public static IDictionary<string, object> Reconfigure()
        {
            CheckIsNotConfigured();
            return ConfigureInternal(t => InvokeReconfigure(t), 
                waitBeforeFirstWrite: false, 
                refreshAppSettings: true,
                settings: null);
        }

        public static async Task<IDictionary<string, object>> ReconfigureAsync()
        {
            CheckIsNotConfigured();
            return await Task.Run(() => ConfigureInternal(t => InvokeReconfigure(t), 
                waitBeforeFirstWrite: true, 
                refreshAppSettings: true,
                settings: null));
        }       
    }
}