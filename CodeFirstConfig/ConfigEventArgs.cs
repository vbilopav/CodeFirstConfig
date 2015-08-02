using System;

namespace CodeFirstConfig
{
    public class ConfigAfterSetEventArgs : ConfigItem
    {
        internal ConfigAfterSetEventArgs(string @namespace, string name, string key, object value)
            : base(@namespace, name, key, value)
        {
        }       
    }

    public class ConfigBeforeSetEventArgs : ConfigItem
    {
        public bool Cancel { get; set; } = false;

        internal ConfigBeforeSetEventArgs(string @namespace, string name, string key, object value)
            : base(@namespace, name, key, value)
        {           
        }
    }

    public class ModelConfiguredEventArgs
    {
        public Type Type { get; }

        internal ModelConfiguredEventArgs(Type type)
        {
            Type = type;
        }
    }

}