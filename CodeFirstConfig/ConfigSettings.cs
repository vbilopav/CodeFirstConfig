using System;
using System.IO;

namespace CodeFirstConfig
{
    public enum ConfigFormat { Json, AppConfig }

    public sealed class ConfigSettings
    {
        private const string DefaultConfigName = ".\\CodeFirstAppSettings.config";
        private ConfigFormat _format = ConfigFormat.AppConfig;

        // protected by ConfigLock
        private static ConfigSettings _instance;
        public static ConfigSettings Instance {
            get { return _instance ?? (_instance = new ConfigSettings()); }
            internal set { _instance = value; }
        }

        public bool SaveConfigFile { get; set; } = true;
        public string SaveConfigFileName { get; set; } = DefaultConfigName;
        public bool EnableFileWatcher { get; set; } = true;

        public ConfigFormat Format
        {
            get { return _format; }
            set
            {
                _format = value;
                if (!Equals(SaveConfigFileName, DefaultConfigName)) return;
                switch (_format)
                {
                    case ConfigFormat.AppConfig:
                        SaveConfigFileName = Path.ChangeExtension(SaveConfigFileName, ".config");
                        break;

                    case ConfigFormat.Json:
                        SaveConfigFileName = Path.ChangeExtension(SaveConfigFileName, ".json");
                        break;
                }
            }
        }

        public bool EnableTimer { get; set; } = false;
        public int? TimerMinutes { get; set; } = 5;
        public int MaxFileWriteRetry { get; set; } = 3;
        public bool ThrowOnConfigureException { get; set; } = true;

        public Action<ConfigAfterSetEventArgs> OnAfterSet { get; set; } = null;
        public Action<ConfigBeforeSetEventArgs> OnBeforeSet { get; set; } = null;
        public Action<ModelConfiguredEventArgs> OnModelConfigured { get; set; } = null;
        public Action<ConfigErrorEventArgs> OnError { get; set; } = null;
    }
}