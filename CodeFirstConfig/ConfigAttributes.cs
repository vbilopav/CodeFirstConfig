using System;

namespace CodeFirstConfig
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConfigSettingsAttribute : Attribute
    {
        public bool Required { get; set; }
        public bool ExecuteAfterSet { get; set; }
        public bool ExecuteBeforeSet { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class ConfigCommentAttribute : Attribute
    {
        public string Comment { get; }
        public ConfigCommentAttribute(string comment) { Comment = comment; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigKeySerializeLevelAttribute : Attribute
    {
        public ushort Level { get; }              
        public ConfigKeySerializeLevelAttribute(ushort level) { Level = level; }
    }    
}