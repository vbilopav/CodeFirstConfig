using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeFirstConfig
{
    internal static class ConfigObjects
    {
        private static readonly IDictionary<string, object> _current;
        private static readonly JsonSerializer _serializer;
        private static DateTime _timeStamp;
        private static NumberFormatInfo _nfi;

        private static void WriteComment(TextWriter writer, ConfigFormat format)
        {
            writer.WriteLine(format == ConfigFormat.AppConfig ? "<!--" : "/*");
            writer.WriteLine("Timestamp: {0:O}", _timeStamp);

            var old = _serializer.Formatting;
            _serializer.Formatting = Formatting.None;
            using (var sw = new StringWriter())
            {                
                writer.Write("App:  ");
                _serializer.Serialize(sw, new
                {
                    Name = App.Config.Name,
                    Id = App.Config.Id,
                    InstanceId = App.Config.InstanceId,
                    Web = App.IsWebApp,
                    Debug = App.IsDebugConfiguration,
                    Debugging = App.Debugging,
                    Testing = App.Testing
                });
                writer.Write(sw.ToString());
            }
            using (var sw = new StringWriter())
            {
                writer.WriteLine();
                writer.Write("Settings:  ");
                _serializer.Serialize(sw, ConfigSettings.Instance);
                writer.Write(sw.ToString());                
            }
            _serializer.Formatting = old;
            writer.WriteLine();
            writer.WriteLine(format == ConfigFormat.AppConfig ? "-->" : "*/");
            writer.WriteLine();
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
            var info = field as PropertyInfo;
            if (info != null && info.GetSetMethod() == null)
            {
                writer.WriteRaw("\t");
                writer.WriteComment("readonly");
            }
            var ca = field.GetCustomAttribute<ConfigCommentAttribute>();
            if (string.IsNullOrEmpty(ca?.Comment)) return;
            writer.WriteRaw("\t");
            writer.WriteComment(ca.Comment);
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
                        var objType = kvp.Value.GetType();

                        var levelAttr = objType.GetCustomAttribute<ConfigKeySerializeLevelAttribute>();
                        string key;
                        if (levelAttr != null)
                        {
                            var keys = kvp.Key.Split('.');
                            key = string.Join(".", keys.Skip(keys.Length - levelAttr.Level));
                            if (string.IsNullOrEmpty(key) || levelAttr.Level == 0) continue;
                        }
                        else
                        {
                            key = kvp.Key;
                        }
                        var configAttr = objType.GetCustomAttribute<ConfigCommentAttribute>();
                        if (!string.IsNullOrEmpty(configAttr?.Comment))
                            writer.WriteComment(configAttr.Comment);                       
                        writer.WritePropertyName(key, false);
                        writer.WriteStartObject();                        
                        foreach (var prop in objType.Properties())
                        {
                            WriteJsonValue(writer, prop, prop.GetValue(kvp.Value, null), prop.PropertyType);
                        }
                        foreach (var field in objType.Fields())
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
            var nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null) type = nullable;
            if (type.IsEnum)
            {
                if (value != null)
                {
                    var e = Enum.GetName(type, value);
                    writer.Write(e ?? value.ToString());
                } else
                    writer.Write("null");
                writer.Write("\" />");
                string enumname = nullable == null ? "\t<!--enum " : "\t<!--nullable enum ";
                string flagsname = nullable == null ? "\t<!--flags " : "\t<!--nullable flags ";
                writer.Write(string.Concat(type.GetCustomAttribute<FlagsAttribute>() != null ? flagsname : enumname,
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
                    if (value == null)
                        writer.Write(string.Concat("null\" />"));
                    else
                    {
                        if (type == typeof(float) || type == typeof(double))
                            writer.Write(string.Concat(((double)value).ToString(_nfi), "\" />"));
                        else
                            writer.Write(string.Concat(value.ToString(), "\" />"));
                    }
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
                if (nullable != null)
                    writer.Write("\t<!--nullable-->");

            }
            var info = field as PropertyInfo;
            if (info != null && info.GetSetMethod() == null)
            {
                writer.Write("\t<!--readonly-->");
            }
            var ca = field.GetCustomAttribute<ConfigCommentAttribute>();
            if (!string.IsNullOrEmpty(ca?.Comment))
            {
                writer.Write(string.Concat("\t<!--", ca.Comment, "-->"));
            }
            writer.WriteLine();
        }

        private static void WriteAppConfigToTextWriter(TextWriter writer, bool skipComment = false)
        {
            _serializer.Formatting = Formatting.None;
            if (!skipComment) WriteComment(writer, ConfigFormat.AppConfig);
            writer.WriteLine("<appSettings>");
            string prevKey = null;
            lock (_current)
            {
                foreach (var kvp in _current)
                {
                    var objType = kvp.Value.GetType();
                    var levelAttr = objType.GetCustomAttribute<ConfigKeySerializeLevelAttribute>();
                    string key;
                    if (levelAttr != null)
                    {
                        var keys = kvp.Key.Split('.');
                        key = string.Join(".", keys.Skip(keys.Length - levelAttr.Level));
                        if (string.IsNullOrEmpty(key) || levelAttr.Level == 0) continue;
                    }
                    else
                    {
                        key = kvp.Key;
                    }
                    if (!string.Equals(key, prevKey))
                    {
                        if (prevKey != null) writer.WriteLine();     
                        var configAttr = objType.GetCustomAttribute<ConfigCommentAttribute>();
                        if (!string.IsNullOrEmpty(configAttr?.Comment))                            
                            writer.WriteLine(string.Concat("\t<!--", configAttr.Comment, "-->"));
                    }                        
                    
                    prevKey = key;
                    foreach (var prop in objType.Properties())
                    {
                        WriteAppConfigValue(writer, prop, prop.GetValue(kvp.Value, null), prop.PropertyType, key);
                    }
                    foreach (var field in objType.Fields())
                    {
                        WriteAppConfigValue(writer, field, field.GetValue(kvp.Value), field.FieldType, key);
                    }
                }
                if (ConfigSettings.Instance.WriteUnbinedAppSettings && ConfigValues.Unbinded.Any())
                {
                    writer.WriteLine();
                    writer.WriteLine("\t<!--Unbinded-->");

                    //maintain original order
                    foreach (var key in ConfigValues.Keys.Where(key => ConfigValues.Unbinded.ContainsKey(key)))
                    {
                        writer.WriteLine($"\t<add key=\"{key}\" value=\"{ConfigValues.Unbinded[key]}\" />");
                    }                                         
                }
            }
            writer.WriteLine("</appSettings>");         
        }

        private static void WriteExceptions(TextWriter writer, ConfigFormat format, List<Exception> exceptions)
        {
            writer.WriteLine(format == ConfigFormat.AppConfig ? "<!--" : "/*");
            var e = new AggregateException(exceptions.ToArray()).ToString();
            if (format == ConfigFormat.AppConfig) e = e.Replace("---", "~~~").Replace("--", "~~");
            writer.Write(e);
            writer.WriteLine(format == ConfigFormat.AppConfig ? "-->" : "*/");
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
                return _current.TryGetValue(key, out config) ? config : null;
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

            if (exceptions == null || !exceptions.Any()) return;
            writer.WriteLine();
            writer.WriteLine();
            WriteExceptions(writer, format, exceptions);
        }

        internal static void ToWriter(TextWriter writer, List<Exception> exceptions = null)
        {
            ToWriter(writer, ConfigSettings.Instance.ConfigFileFormat, exceptions: exceptions);
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
            _nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
        }
    }
}
    