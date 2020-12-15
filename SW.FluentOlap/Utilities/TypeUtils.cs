using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return Nullable.GetUnderlyingType(type) != null ||
                   type == typeof(string) || type == typeof(DateTime) ||
                   type == typeof(decimal) || type == typeof(long) ||
                   type == typeof(TimeSpan) || type == typeof(double) ||
                    type.IsPrimitive;
        }

        public static bool TryGuessInternalType(Type type, out InternalType sqlType)
        {
            Type focusedType = Nullable.GetUnderlyingType(type) ?? type;
            sqlType = new InternalType();

            if (focusedType.IsAssignableFrom(typeof(string)) || focusedType.IsAssignableFrom(typeof(TimeSpan)))
            {
                sqlType = InternalType.STRING;
                return true;
            }

            if (focusedType.IsAssignableFrom(typeof(DateTime)))
            {
                sqlType = InternalType.DATETIME;
                return true;
            }

            if (focusedType.IsAssignableFrom(typeof(double)) || focusedType.IsAssignableFrom(typeof(decimal)))
            {
                sqlType = InternalType.FLOAT;
                return true;
            }

            if (!focusedType.IsPrimitive) return false;

            if (focusedType.IsAssignableFrom(typeof(float)))
                sqlType = InternalType.FLOAT;

            if (focusedType.IsAssignableFrom(typeof(int)) || focusedType.IsAssignableFrom(typeof(long)))
                sqlType = InternalType.INTEGER;

            if (focusedType.IsAssignableFrom(typeof(bool)))
                sqlType = InternalType.BOOLEAN;


            return true;
        }
    }
}