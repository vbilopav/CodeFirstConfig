using System.IO;

namespace CodeFirstConfig
{
    public enum ConfigFormat { Json, AppConfig }

    public class ConfigSettingsManager : ConfigManager<ConfigSettings> { }

    [ConfigComment("Config settings")]
    public sealed class ConfigSettings
    {
        public static ConfigSettings Config { get { return ConfigSettingsManager.Config; } }
        public bool SaveConfigFile { get; set; }
        public string SaveConfigFileName { get; set; }
        public bool EnableFileWatcher { get; set; }
        public ConfigFormat ConfigFormat { get; set; }
        public bool EnableTimer { get; set; }
        public int? TimerMinutes { get; set; }
        public int MaxFileWriteRetry { get; set; }
        public bool ThrowOnConfigureException { get; set; }

        public ConfigSettings()
        {
            SaveConfigFile = true;
            SaveConfigFileName = Path.Combine(App.Config.DataFolder, "AppSettings.config");
            ConfigFormat = ConfigFormat.AppConfig;
            EnableFileWatcher = true;
            EnableTimer = false;
            TimerMinutes = 5;
            MaxFileWriteRetry = 3;
            ThrowOnConfigureException = true;
        }
    }
}