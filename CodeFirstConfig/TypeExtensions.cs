using System;

namespace CodeFirstConfig
{
    internal static class TypeExtensions
    {
        internal static bool IsSubclassOfRawGeneric(this Type type, Type generic)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == cur) return true;
                type = type.BaseType;
            }
            return false;
        }

        internal static bool IsSimpleType(this Type type) =>
                type == typeof(string) ||
                type == typeof(int) ||
                type == typeof(int?) ||
                type == typeof(DateTime) ||
                type == typeof(DateTime?) ||
                type == typeof(bool) ||
                type == typeof(bool?);        
    }
}