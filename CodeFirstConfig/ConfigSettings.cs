using System.IO;

namespace CodeFirstConfig
{
    public enum ConfigFormat { Json, AppConfig }

    public sealed class ConfigSettings
    {
        private const string DefaultConfigName = ".\\CodeFirstAppSettings.config";
        private ConfigFormat _format = ConfigFormat.AppConfig;

        // protected by ConfigLock
        public static ConfigSettings Instance { get; internal set; }

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
    }
}