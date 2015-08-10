using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    internal static class ConfigValues
    {
        private static IEnumerable<string> _keys;
        private static IDictionary<string, string> _dictionary;
        private static IDictionary<string, string> _unbinded;
        private static IDictionary<Type, IEnumerable<PropertyInfo>> _props;
        private static IDictionary<Type, IEnumerable<FieldInfo>> _fields;

        internal static IDictionary<string, string> GetConfiguration()
        {
            for (int retry = 0; retry < ConfigSettings.MaxRetryBeforeException; retry++)
            {
                try
                {
                    _keys = ConfigurationManager
                                .AppSettings
                                .AllKeys;
                    return _keys.OrderByDescending(a => a.Length).ToDictionary(a => a, a => ConfigurationManager.AppSettings[a]);
                }
                catch (ConfigurationErrorsException)
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
            return null;
        }

        internal static IEnumerable<KeyValuePair<string, string>> Dictionary
        {
            get
            {
                if (_dictionary != null) return _dictionary;
                lock (Configurator.ConfigLock)
                {
                    if (_dictionary != null) return _dictionary;
                    Reconfigure(false);
                    return _dictionary;
                }
            }
        }

        internal static IDictionary<string, string> Unbinded { get { return _unbinded; } }  //protected by ConfigLocks.ConfigLock

        internal static IEnumerable<string> Keys { get { return _keys; } }  //protected by ConfigLocks.ConfigLock

        internal static void AddUbined(string key, string value)      //protected by ConfigLocks.ConfigLock
        {            
            _unbinded[key] = value;
        }

        internal static void SetProperties(Type type, IEnumerable<PropertyInfo> props)  //protected by ConfigLocks.ConfigLock
        {
            _props[type] = props;
        }

        internal static void SetFields(Type type, IEnumerable<FieldInfo> fields)  //protected by ConfigLocks.ConfigLock
        {
            _fields[type] = fields;
        }

        internal static IEnumerable<PropertyInfo> Properties(this Type type)  //protected by ConfigLocks.ConfigLock
        {
            return _props[type];
        }

        internal static IEnumerable<FieldInfo> Fields(this Type type)  //protected by ConfigLocks.ConfigLock
        {
            return _fields[type];
        }

        internal static DbConfigOptions DatabaseOptions { get; set; } //protected by ConfigLocks.ConfigLock


        internal static void Reconfigure(bool refreshAppSettings) //protected by ConfigLocks.ConfigLock
        {           
            if (refreshAppSettings) ConfigurationManager.RefreshSection("appSettings");
            if (DatabaseOptions != null)
            {
                _dictionary = GetConfiguration();
                _dictionary = DatabaseOptions.DbConfigure(_dictionary);
            }
            else
            {
                _dictionary = GetConfiguration();
            }
            _unbinded = new Dictionary<string, string>();
        }

        internal static async Task ReconfigureAsync(bool refreshAppSettings) //protected by ConfigLocks.ConfigLock
        {           
            if (refreshAppSettings) ConfigurationManager.RefreshSection("appSettings");
            if (DatabaseOptions != null)
            {
                var d = GetConfiguration();
                d = await DatabaseOptions.DbConfigureAsync(d);
                _dictionary = d;
            }
            else
            {
                _dictionary = GetConfiguration();
            }
            _unbinded = new Dictionary<string, string>();
        }

        static ConfigValues()
        {
            _dictionary = null;
            _props = new Dictionary<Type, IEnumerable<PropertyInfo>>();
            _fields = new Dictionary<Type, IEnumerable<FieldInfo>>();
        }
    }
}