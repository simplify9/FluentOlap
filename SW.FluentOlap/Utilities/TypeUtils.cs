using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Utilities
{
    internal static class TypeUtils
    {
        public static InternalType GuessType(Type type)
        {
            InternalType t;
            if (!TryGuessInternalType(type, out t))
                throw new Exception($"Could not guess type of {type.Name}, please define using Property()");
            return t;
        }


        public static bool IsPrimitive(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
                return true;
            if (type == typeof(string))
                return true;
            if (type == typeof(DateTime))
                return true;
            if (type == typeof(decimal))
                return true;
            return type.IsPrimitive;
        }
        
        public static bool TryGuessInternalType(Type type, out InternalType sqlType)
        {
            sqlType = new InternalType();

            if (type.IsAssignableFrom(typeof(string)))
            {
                sqlType = InternalType.STRING;
                return true;
            }

            if (type.IsAssignableFrom(typeof(DateTime)))
            {
                sqlType = InternalType.DATETIME;
                return true;
            }

            if (type.IsAssignableFrom(typeof(decimal)))
            {
                sqlType = InternalType.FLOAT;
                return true;
            }
            

            if (!type.IsPrimitive) return false;

            if (type.IsAssignableFrom(typeof(float)))
                sqlType = InternalType.FLOAT;

            if (type.IsAssignableFrom(typeof(int)))
                sqlType = InternalType.INTEGER;

            if (type.IsAssignableFrom(typeof(bool)))
                sqlType = InternalType.BOOLEAN;


            return true;

        }
    }
}
