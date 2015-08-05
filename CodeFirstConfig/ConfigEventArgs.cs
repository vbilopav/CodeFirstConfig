using System;
using System.Runtime.Serialization;

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
        public object Model { get; }

        internal ModelConfiguredEventArgs(Type type, object model)
        {
            Type = type;
            Model = model;
        }
    }

    public class ConfigErrorEventArgs
    {
        public Exception Exception { get; }

        internal ConfigErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }

}