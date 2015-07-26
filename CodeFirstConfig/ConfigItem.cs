namespace CodeFirstConfig
{
    public class ConfigItem
    {
        public string Namespace { get; }
        public string Name { get; }
        public string Key { get; }
        public object Value { get; }

        public ConfigItem(string @namespace, string name, string key, object value)
        {
            Namespace = @namespace;
            Name = name;
            Key = key;
            Value = value;
        }
    }
}