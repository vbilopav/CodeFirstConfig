using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CodeFirstConfig
{
    internal class ModelConfigurator<TModel> where TModel : class, new()
    {
        private readonly IDictionary<string, PropertyInfo> _props;
        private readonly IDictionary<string, FieldInfo> _fields;
        private readonly HashSet<string> _requireds;
        private readonly string _namespace;
        private readonly string[] _ns;
        private readonly Type _type;
        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs() => ConfigValues.Dictionary;


        private string GetKeyName(string key)
        {
            string[] keys = key.Split('.');            
            //compare from root
            if (keys.Length == 1) return key;
            if (keys.Length > _ns.Length + 1) return null;

            //key only haz classname.key
            if (keys.Length == 2 && _ns.Length > 0)
            {
                if (string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]))
                    return keys[keys.Length - 1];
            }
            //key only haz namespace.classname.key
            if (keys.Length == 3 && _ns.Length > 1)
            {
                if (
                    string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]) &&
                    string.Equals(keys[keys.Length - 3], _ns[_ns.Length - 2])
                    )
                    return keys[keys.Length - 1];
            }
            //key only haz namespace1.namespace2.classname.key
            if (keys.Length == 4 && _ns.Length > 2)
            {
                if (
                    string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]) &&
                    string.Equals(keys[keys.Length - 3], _ns[_ns.Length - 2]) &&
                    string.Equals(keys[keys.Length - 4], _ns[_ns.Length - 3])
                    )
                    return keys[keys.Length - 1];
            }
            //key only haz namespace1.namespace2.namespace3.classname.key
            if (keys.Length == 5 && _ns.Length > 3)
            {
                if (
                    string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]) &&
                    string.Equals(keys[keys.Length - 3], _ns[_ns.Length - 2]) &&
                    string.Equals(keys[keys.Length - 4], _ns[_ns.Length - 3]) &&
                    string.Equals(keys[keys.Length - 5], _ns[_ns.Length - 4])
                    )
                    return keys[keys.Length - 1];
            }
            //key only haz
            //namespace1.namespace2.namespace3.namespace4.classname.key
            if (keys.Length == 6 && _ns.Length > 4)
            {
                if (
                    string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]) &&
                    string.Equals(keys[keys.Length - 3], _ns[_ns.Length - 2]) &&
                    string.Equals(keys[keys.Length - 4], _ns[_ns.Length - 3]) &&
                    string.Equals(keys[keys.Length - 5], _ns[_ns.Length - 4]) &&
                    string.Equals(keys[keys.Length - 6], _ns[_ns.Length - 5])
                    )
                    return keys[keys.Length - 1];
            }
            //namespace1.namespace2.namespace3.namespace4.classname5.classname.key
            if (keys.Length == 6 && _ns.Length > 4)
            {
                if (
                    string.Equals(keys[keys.Length - 2], _ns[_ns.Length - 1]) &&
                    string.Equals(keys[keys.Length - 3], _ns[_ns.Length - 2]) &&
                    string.Equals(keys[keys.Length - 4], _ns[_ns.Length - 3]) &&
                    string.Equals(keys[keys.Length - 5], _ns[_ns.Length - 4]) &&
                    string.Equals(keys[keys.Length - 6], _ns[_ns.Length - 5]) &&
                    string.Equals(keys[keys.Length - 7], _ns[_ns.Length - 6])
                    )
                    return keys[keys.Length - 1];
            }
            return null;
        }


        private ConfigBeforeSetEventArgs ExecuteOnBeforeSetAction(ConfigSettingsAttribute attr, string name, string key, object value)
        {
            if (attr == null || !attr.ExecuteBeforeSet) return null;
            if (ConfigSettings.Instance.OnBeforeSet == null)
                throw new CodeFirstConfigException(
                    $"Before set config action on key '{key}' could not be executed because it is not configured in global config manager -> Configurator.OnBeforeSet",
                    new ConfigItem(_namespace, name, key, value));
            var args = new ConfigBeforeSetEventArgs(_namespace, name, key, value);
            ConfigSettings.Instance.OnBeforeSet(args);
            return args;
        }

        private void ExecuteOnAfterSetAction(ConfigSettingsAttribute attr, string name, string key, object value)
        {
            if (attr == null || !attr.ExecuteAfterSet) return;
            if (ConfigSettings.Instance.OnAfterSet == null)
                throw new CodeFirstConfigException(
                    $"After set config action on key '{key}' could not be executed because it is not configured in global config manager -> Configurator.OnAfterSet",
                    new ConfigItem(_namespace, name, key, value));
            ConfigSettings.Instance.OnAfterSet(new ConfigAfterSetEventArgs(_namespace, name, key, value));
        }

        private object ParseModelValue(Type type, string value, string name = null, string key = null)
        {
            try
            {
                if (value == null) return null;
                if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase)) return null;
                if (type == typeof (string)) return Convert.ToString(value);
                if (type == typeof (object)) return value;
                type = Nullable.GetUnderlyingType(type) ?? type;
                if (type == typeof (bool)) return Convert.ToBoolean(value);
                if (type == typeof (DateTime)) return Convert.ToDateTime(value);
                if (type.IsEnum) return (Enum.Parse(type, value));
                if (type == typeof (ulong)) return Convert.ToUInt64(value);
                if (type == typeof (long)) return Convert.ToInt64(value);
                if (type == typeof (uint)) return Convert.ToUInt32(value);
                if (type == typeof (int)) return Convert.ToInt32(value);
                if (type == typeof (ushort)) return Convert.ToUInt16(value);
                if (type == typeof (short)) return Convert.ToInt16(value);
                if (type == typeof (double)) return double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                if (type == typeof (float)) return float.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                if (type == typeof (decimal))
                    return decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                if (type == typeof (char)) return Convert.ToChar(value);
                if (type == typeof (byte)) return Convert.ToByte(value);
                if (type == typeof (sbyte)) return Convert.ToSByte(value);
                return JsonConvert.DeserializeObject(value, type);
            }
            catch (Exception e)
            {
                throw new CodeFirstConfigException(
                    $"Value \"{value}\" of type {type.Name} could not be parsed!",
                    new ConfigItem(_namespace, name, key, value), 
                    e);
            }
        }

        private bool SetValueToModel(string name, string key, string value, ref TModel model)
        {
            PropertyInfo prop;
            ConfigSettingsAttribute attr;
            ConfigBeforeSetEventArgs args;
            object val;
            if (_props.TryGetValue(name, out prop))
            {
                attr = prop.GetCustomAttribute<ConfigSettingsAttribute>();
                val = ParseModelValue(prop.PropertyType, value, name, key);
                args = ExecuteOnBeforeSetAction(attr, name, key, val);
                if (args != null && args.Cancel) return false;
                prop.SetValue(model, val);
                ExecuteOnAfterSetAction(attr, name, key, val);                             
                return true;
            }
            FieldInfo field;                       
            if (!_fields.TryGetValue(name, out field)) return false;
            attr = field.GetCustomAttribute<ConfigSettingsAttribute>();
            val = ParseModelValue(field.FieldType, value, name, key);
            args = ExecuteOnBeforeSetAction(attr, name, key, val);                
            if (args != null && args.Cancel) return false;
            field.SetValue(model, val);
            ExecuteOnAfterSetAction(attr, name, key, val);
            return true;
        }

        private TModel Build(TModel model)
        {            
            var requireds = new List<string>();               
            var used = new HashSet<string>();
            foreach (var item in GetKeyValuePairs())
            {
                string name = GetKeyName(item.Key);
                if (name == null) continue;
                if (used.Contains(name)) continue;               
                if (SetValueToModel(name, item.Key, item.Value, ref model))
                {
                    used.Add(name);                    
                    if (_requireds.Contains(name))
                        requireds.Add(name);
                } else
                {
                    if (ConfigSettings.Instance.WriteUnbinedAppSettings)
                        ConfigValues.AddUbined(item.Key, item.Value);
                }            
            }
            if (requireds.Count != _requireds.Count)
            {
                throw new CodeFirstConfigException(
                    $"Config exception! Some of required config elements in namespace {_namespace} are required and not found in any configuration! List of required elements: {string.Join(",", _requireds.Except(requireds))}",
                    new ConfigItem(@namespace: _namespace, name: null, key: null, value: null));
            }
            return model;
        }

        public TModel ConfigureModel()
        {
            TModel model = new TModel();
            try
            {
                ConfigObjects.Set(_namespace, Build(model));
            }
            catch (Exception e)
            {
                ConfigObjects.Set(_namespace, model);
                if (ConfigSettings.Instance.ThrowOnConfigureException) throw;
                Configurator.AddExceptions(e);
            }
            return model;
        }

        public static string GetNamespace(Type type) => 
            (type.IsGenericType ? type.FullName.Substring(0, type.FullName.IndexOf('`')) : type.FullName).Replace('+', '.');   
        
        public ModelConfigurator()
        {
            _type = typeof(TModel);
            _namespace = GetNamespace(_type);
            _ns = _namespace.Split(new[]{'.'}, StringSplitOptions.RemoveEmptyEntries);

            _props = _type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                .Where(p => p.CanWrite)
                .Where(p => p.GetSetMethod(true).IsPublic)
                .ToDictionary(p => p.Name, p => p);
            _fields = _type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField)
                .Where(f => f.IsPublic)
                .ToDictionary(f => f.Name, f => f);
            ConfigValues.SetProperties(_type, _props.Values);
            ConfigValues.SetFields(_type, _fields.Values);
            _requireds = new HashSet<string>(
               _props
                   .Where(f => f.Value.IsDefined(typeof(ConfigSettingsAttribute), false))
                   .Where(f => f.Value.GetCustomAttribute<ConfigSettingsAttribute>().Required)
                   .Select(f => f.Key)
                   .Concat(
                       _fields
                           .Where(f => f.Value.IsDefined(typeof(ConfigSettingsAttribute), false))
                           .Where(f => f.Value.GetCustomAttribute<ConfigSettingsAttribute>().Required)
                           .Select(f => f.Key)
                   )
                   .ToList());
        }       
    }
}