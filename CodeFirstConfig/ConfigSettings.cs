using Newtonsoft.Json;
using System;
using System.IO;

namespace CodeFirstConfig
{
    public enum ConfigFormat { Json, AppConfig }

    public class ConfigSettings
    {
        internal const int MaxRetryBeforeException = 5;
        internal const int DelayReadWriteLockedFileAfterExceptionMs = 250;

        private const string DefaultConfigName = ".\\CodeFirstAppSettings.config";
        private ConfigFormat _format = ConfigFormat.AppConfig;
       
        public bool SaveConfigFile { get; set; } = false;
        public string ConfigFileName { get; set; } = DefaultConfigName;
        public bool WriteUnbinedAppSettings { get; set; } = true;
        public bool EnableFileWatcher { get; set; } = true;

        public ConfigFormat ConfigFileFormat
        {
            get { return _format; }
            set
            {
                _format = value;
                if (!Equals(ConfigFileName, DefaultConfigName)) return;
                switch (_format)
                {
                    case ConfigFormat.AppConfig:
                        ConfigFileName = Path.ChangeExtension(ConfigFileName, ".config");
                        break;

                    case ConfigFormat.Json:
                        ConfigFileName = Path.ChangeExtension(ConfigFileName, ".json");
                        break;
                }
            }
        }

        public bool EnableTimer { get; set; } = false;
        public int? TimerMinutes { get; set; } = 5;
        public bool ThrowOnConfigureException { get; set; } = false;

        // protected by ConfigLock
        private static ConfigSettings _instance;
        public static ConfigSettings Instance
        {
            get { return _instance ?? (_instance = new ConfigSettings()); }
            internal set { _instance = value; }
        }

        [JsonIgnore]
        public Action<ConfigAfterSetEventArgs> OnAfterSet { get; set; } = null;
        [JsonIgnore]
        public Action<ConfigBeforeSetEventArgs> OnBeforeSet { get; set; } = null;
        [JsonIgnore]
        public Action<ModelConfiguredEventArgs> OnModelConfigured { get; set; } = null;
        [JsonIgnore]
        public Action<ConfigErrorEventArgs> OnError { get; set; } = null;
    }   
}