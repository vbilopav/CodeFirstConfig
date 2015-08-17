using System;

namespace CodeFirstConfig
{
    internal static class Extensions
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

        internal static bool One(this string s, char ch)
        {
            int count = 0;
            foreach (char c in s)
            {
                if (c == ch) count++;
                if ((count) >= 2)
                    return false;
            }
            return count == 1;
        }
    }
}