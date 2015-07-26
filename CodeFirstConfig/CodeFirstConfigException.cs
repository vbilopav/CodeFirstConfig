using System;

namespace CodeFirstConfig
{
    public class CodeFirstConfigException : Exception
    {
        public ConfigItem Item { get; }

        public CodeFirstConfigException(string message, ConfigItem item = null) : base(message)
        {
            Item = item;
        }
    }
}