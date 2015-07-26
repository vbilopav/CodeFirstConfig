namespace CodeFirstConfig
{
    public class ConfigAfterSetEventArgs : ConfigItem
    {
        public ConfigAfterSetEventArgs(string @namespace, string name, string key, object value)
            : base(@namespace, name, key, value)
        {
        }
       
    }

    public class ConfigBeforeSetEventArgs : ConfigItem
    {
        public bool Cancel { get; set; } = false;

        public ConfigBeforeSetEventArgs(string @namespace, string name, string key, object value)
            : base(@namespace, name, key, value)
        {           
        }
    }
}