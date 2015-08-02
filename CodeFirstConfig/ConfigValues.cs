using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    internal class ConfigValues
    {
        private static IDictionary<string, string> _dictionary;
       
        internal static IDictionary<string, string> GetConfiguration() =>
            ConfigurationManager
                        .AppSettings
                        .AllKeys                        
                        .OrderByDescending(a => a.Length)
                        .ToDictionary(a => a, a => ConfigurationManager.AppSettings[a]); 
               
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
        }

        static ConfigValues()
        {
            _dictionary = null;
        }
    }
}