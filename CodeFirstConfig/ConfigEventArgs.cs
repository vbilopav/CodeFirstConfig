namespace CodeFirstConfig
{
    public class ConfigAfterSetEventArgs
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }

        public ConfigAfterSetEventArgs()
        {           
        }

        public ConfigAfterSetEventArgs(string @namespace, string name, string key, object value)
        {
            Namespace = @namespace;
            Name = name;
            Key = key;
            Value = value;
        }
    }

    public class ConfigBeforeSetEventArgs : ConfigAfterSetEventArgs
    {
        public bool Cancel { get; set; }

        public ConfigBeforeSetEventArgs()
        {
            Cancel = false;
        }

        public ConfigBeforeSetEventArgs(string @namespace, string name, string key, object value)
            : base(@namespace, name, key, value)
        {
            Cancel = false;
        }
    }
}