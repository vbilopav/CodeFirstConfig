using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using CodeFirstConfig;
using Newtonsoft.Json;

namespace CodeFirstConfig
{
    public class ModelConfigurator<TModel> where TModel : class, new()
    {
        private readonly IDictionary<string, PropertyInfo> _props;
        private readonly IDictionary<string, FieldInfo> _fields;
        private readonly HashSet<string> _requireds;
        private readonly string _namespace;
        private readonly string[] _ns;

        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            return ConfigValues.Dictionary;
        }

        private static object ParseModelValue(Type type, object value)
        {            
            if (type.IsSimpleType())
                return type == typeof(string) ? (value == null ? null : Convert.ToString(value)) : value;
            return type.IsEnum ? Enum.Parse(type, value as string) : JsonConvert.DeserializeObject(value as string, type);                             
        }

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


        private ConfigBeforeSetEventArgs ExecuteOnBeforeSetAction(MemberInfo member, ConfigSettingsAttribute attr, string name, string key, object value)
        {
            if (attr != null && attr.ExecuteBeforeSet)
            {
                if (Configurator.OnBeforeSet == null)
                    throw new ArgumentException(
                        $"Before set config action on key '{key}' could not be executed because it is not configured in GlobalConfigManager!");
                var args = new ConfigBeforeSetEventArgs(_namespace, name, key, value);
                Configurator.OnBeforeSet(args);
                return args;
            }
            return new ConfigBeforeSetEventArgs();
        }

        private void ExecuteOnAfterSetAction(MemberInfo member, ConfigSettingsAttribute attr, string name, string key, object value)
        {                           
            if (attr != null && attr.ExecuteAfterSet)
            {
                if (Configurator.OnAfterSet == null)
                    throw new ArgumentException(
                        $"After set config action on key '{key}' could not be executed because it is not configured in GlobalConfigManager!");                   
                Configurator.OnAfterSet(new ConfigAfterSetEventArgs(_namespace, name, key, value));    
            }       
        }
        
        private void SetValueToModel(string name, string key, object value, ref TModel model)
        {
            PropertyInfo prop;
            ConfigSettingsAttribute attr;
            ConfigBeforeSetEventArgs args;
            if (_props.TryGetValue(name, out prop))
            {
                attr = prop.GetCustomAttribute<ConfigSettingsAttribute>();
                value = ParseModelValue(prop.PropertyType, value);
                args = ExecuteOnBeforeSetAction(prop, attr, name, key, value);
                if (args.Cancel) return;
                prop.SetValue(model, value);
                ExecuteOnAfterSetAction(prop, attr, name, key, value);                             
                return;
            }
            FieldInfo field;                       
            if (!_fields.TryGetValue(name, out field)) return;
            attr = field.GetCustomAttribute<ConfigSettingsAttribute>();            
            value = ParseModelValue(field.FieldType, value);
            args = ExecuteOnBeforeSetAction(field, attr, name, key, value);                
            if (args.Cancel) return;
            field.SetValue(model, value);
            ExecuteOnAfterSetAction(field, attr, name, key, value);   
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
                string entry = item.Value;
                object val;
                if (string.Equals(entry, "null", StringComparison.OrdinalIgnoreCase))
                    val = null;
                else
                {
                    DateTime dateTimeVal;
                    if (DateTime.TryParse(entry, out dateTimeVal))
                        val = dateTimeVal;
                    else
                    {
                        bool boolVal;
                        //try bool      
                        if (bool.TryParse(entry, out boolVal))
                            val = boolVal;
                        else
                        {
                            //try int
                            int intVal;
                            if (int.TryParse(entry, out intVal))
                                val = intVal;
                            else
                                //string it is if all fails
                                val = entry;
                        }
                    }
                }
                SetValueToModel(name, item.Key, val, ref model);
                used.Add(name);
                if (_requireds.Contains(name))
                    requireds.Add(name);                
            }
            if (requireds.Count != _requireds.Count)
            {
                throw new ConfigurationErrorsException(
                    $"Config exception! Some of required config elements in namespace {_namespace} are required and not found in any configuration! List of required elements: {string.Join(",", _requireds.Except(requireds))}");
            }
            return model;
        }

        public TModel ConfigureModel()
        {
            TModel model;
            var current = ConfigObjects.Get(_namespace);
            if (current == null)
                model = new TModel();
            else
                model = (TModel)current;            
            ConfigObjects.Set(_namespace, Build(model));
            return model;
        }

        public static string GetNamespace(Type type)
        {
            return
                (type.IsGenericType ?
                type.FullName.Substring(0, type.FullName.IndexOf('`')) :
                type.FullName).Replace('+', '.');   
        }

        public ModelConfigurator()
        {
            Type type = typeof(TModel);
            _namespace = GetNamespace(type);
            _ns = _namespace.Split(new[]{'.'}, StringSplitOptions.RemoveEmptyEntries);
            _props = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                .Where(ø => ø.CanWrite)
                .Where(ø => ø.GetSetMethod(true).IsPublic)
                .ToDictionary(ø => ø.Name, ø => ø);
            _fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField)
                .Where(ø => ø.IsPublic)
                .ToDictionary(ø => ø.Name, ø => ø);
            _requireds = new HashSet<string>(
               _props
                   .Where(ø => ø.Value.IsDefined(typeof(ConfigSettingsAttribute), false))
                   .Where(ø => ø.Value.GetCustomAttribute<ConfigSettingsAttribute>().Required)
                   .Select(ø => ø.Key)
                   .Concat(
                       _fields
                           .Where(ø => ø.Value.IsDefined(typeof(ConfigSettingsAttribute), false))
                           .Where(ø => ø.Value.GetCustomAttribute<ConfigSettingsAttribute>().Required)
                           .Select(ø => ø.Key)
                   )
                   .ToList());
        }       
    }
}