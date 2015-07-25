using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;


namespace CodeFirstConfig
{
    internal static class ConfigObjects
    {
        private static readonly IDictionary<string, object> _current;
        private static readonly JsonSerializer _serializer;
        private static DateTime _timeStamp;

        private static void WriteComment(TextWriter writer, ConfigFormat format)
        {
            writer.WriteLine(format == ConfigFormat.AppConfig ? "<!--" : "/*");
            writer.WriteLine("\tApplication:        {0}", App.Config.Name);
            writer.WriteLine("\tTimestamp:          {0:O}", _timeStamp);
            writer.WriteLine("\tThread:             [{0}:{1}:{2}]",                 
                Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.CurrentCulture,
                Thread.CurrentThread.CurrentUICulture
                );
            writer.WriteLine("\tKeys:               {0}", string.Join(", ", _current.Keys));
            if (writer is StreamWriter)
            {
                writer.WriteLine("\tFile:               {0}", ConfigSettings.Config.SaveConfigFileName);                
            }
            writer.WriteLine(format == ConfigFormat.AppConfig ? "-->\n" : "*/\n");
        }

        
        private static void WriteJsonValue(JsonWriter writer, MemberInfo field, object value, Type type)
        {
            writer.WritePropertyName(field.Name);            
            if (type.IsEnum)
            {
                var e = Enum.GetName(type, value);
                if (value is DateTime)
                    writer.WriteValue(e ?? ((DateTime)value).ToString("yyyy-MM-dd"));
                else
                    writer.WriteValue(e ?? value.ToString());
                writer.WriteRaw("\t");
                writer.WriteComment(
                    string.Concat(type.GetCustomAttribute<FlagsAttribute>() != null ? "flags " : "enum ", 
                    type.Name, " { ",
                    string.Join(", ", Enum.GetNames(type)), " }"));
            }
            else
            {
                if (type.IsPrimitive)                
                    writer.WriteValue(value);
                else
                    _serializer.Serialize(writer, value);
            }
            if (field is PropertyInfo && (field as PropertyInfo).GetSetMethod() == null)
            {
                writer.WriteRaw("\t");
                writer.WriteComment("readonly");
            }
            var ca = field.GetCustomAttribute<ConfigCommentAttribute>();
            if (ca != null && !string.IsNullOrEmpty(ca.Comment))
            {
                writer.WriteRaw("\t");
                writer.WriteComment(ca.Comment);
            }
        }
        
        private static void WriteJsonToTextWriter(TextWriter textWriter, bool skipComment = false)
        {
            _serializer.Formatting = Formatting.Indented;
            if (!skipComment) WriteComment(textWriter, ConfigFormat.Json);
            using (JsonWriter writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();
                lock (_current)
                {
                    foreach (var kvp in _current)
                    {
                        writer.WritePropertyName(kvp.Key, false);
                        writer.WriteStartObject();
                        var objType = kvp.Value.GetType();
                        foreach (var prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                        {
                            WriteJsonValue(writer, prop, prop.GetValue(kvp.Value, null), prop.PropertyType);
                        }
                        foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        {
                            WriteJsonValue(writer, field, field.GetValue(kvp.Value), field.FieldType);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
            }
        }
        
        private static void WriteAppConfigValue(TextWriter writer, MemberInfo field, object value, Type type, string key)
        {
            writer.Write("\t<add key=\"");
            writer.Write(string.Concat(key, ".", field.Name));
            writer.Write("\" value=\"");
            if (type.IsEnum)
            {
                var e = Enum.GetName(type, value);
                writer.Write(e ?? value.ToString());
                writer.Write("\" />");
                writer.Write(string.Concat(type.GetCustomAttribute<FlagsAttribute>() != null ? "\t<!--flags " : "\t<!--enum ",
                    type.Name, " { ",                    
                    string.Join(", ", Enum.GetNames(type)), " }",
                    "-->"));
            }
            else
            {
                if (type.IsPrimitive || type == typeof(string))
                {
                    if (type == typeof(string) && value != null)
                        value = ((string)value).Replace("\r\n", "");
                    if (value == null) value = "null";
                    writer.Write(string.Concat(value.ToString(), "\" />"));
                }
                else
                {
                    using (var sw = new StringWriter())
                    {
                        _serializer.Serialize(sw, value, type);
                        writer.Write(sw.ToString().Replace('\"', '\''));
                    }                    
                    writer.Write("\" />");
                }                
            }
            if (field is PropertyInfo && (field as PropertyInfo).GetSetMethod() == null)
            {
                writer.Write("\t<!--readonly-->");
            }
            var ca = field.GetCustomAttribute<ConfigCommentAttribute>();
            if (ca != null && !string.IsNullOrEmpty(ca.Comment))
            {
                writer.Write(string.Concat("\t<!--", ca.Comment, "-->"));
            }
            writer.WriteLine();
        }

        private static void WriteAppConfigToTextWriter(TextWriter writer, bool skipComment = false)
        {
            _serializer.Formatting = Formatting.None;
            if (!skipComment) WriteComment(writer, ConfigFormat.AppConfig);
            //writer.WriteLine("/*");
            writer.WriteLine("<appSettings>");
            string prevKey = null;
            lock (_current)
            {
                foreach (var kvp in _current)
                {
                    var objType = kvp.Value.GetType();
                    
                    if (!string.Equals(kvp.Key, prevKey))
                    {
                        if (prevKey != null)
                            writer.WriteLine();
                        var attr = kvp.Value.GetType().GetCustomAttribute<ConfigCommentAttribute>();
                        if (attr != null && !string.IsNullOrEmpty(attr.Comment))                            
                            writer.WriteLine(string.Concat("\t<!--", attr.Comment, "-->"));
                    }                        
                    
                    prevKey = kvp.Key;
                    foreach (var prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        WriteAppConfigValue(writer, prop, prop.GetValue(kvp.Value, null), prop.PropertyType, kvp.Key);
                    }
                    foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    {
                        WriteAppConfigValue(writer, field, field.GetValue(kvp.Value), field.FieldType, kvp.Key);
                    }
                }
            }
            writer.WriteLine("</appSettings>");
            //writer.WriteLine("*/");            
        }

        internal static IDictionary<string, object> Current { get { lock (_current) return _current; } }

        internal static void Set(string key, object config)
        {
            lock (_current) _current[key] = config;
        }

        internal static object Get(string key)
        {            
            lock (_current) 
            {
                object config;
                if (_current.TryGetValue(key, out config))
                    return config;
                return null;
            }
        }

        internal static void SetTimeStamp() //protected by ConfigLock
        {
            _timeStamp = DateTime.Now;
        }

        internal static void ToWriter(TextWriter writer, ConfigFormat format, bool skipComment = false, List<Exception> exceptions = null)
        {
            if (format == ConfigFormat.Json)
                WriteJsonToTextWriter(writer, skipComment);
            if (format == ConfigFormat.AppConfig)
                WriteAppConfigToTextWriter(writer, skipComment);

            if (exceptions != null && exceptions.Any())
            {
                writer.Write("\n\n");
                writer.Write(new AggregateException(exceptions.ToArray()).ToString());
            }                
        }

        internal static void ToWriter(TextWriter writer, List<Exception> exceptions = null)
        {
            ToWriter(writer, ConfigSettings.Config.ConfigFormat, exceptions: exceptions);
        }

        internal static void ToFile(string fileName, List<Exception> exceptions = null)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName)) 
            {
                try
                {
                    ToWriter(streamWriter, exceptions: exceptions);
                }
                finally
                {
                    streamWriter.Close();
                }                                
            }
        }

        static ConfigObjects()
        {
            _current = new SortedDictionary<string, object>();
            _serializer = new JsonSerializer
            {                
                ContractResolver = new DefaultContractResolver()
            };            
            _serializer.Converters.Add(new StringEnumConverter());
            //_serializer.Converters.Add(new EscapeQuoteConverter());
        }
    }
}
    