using System;

namespace SW.FluentOlap.Utilities
{
    internal static class MasterWrappers
    {
        public static object MasterFunctionWrapper<T>(Func<T, T> func, object o, object defaultValue)
        {
            if (o == null && typeof(T).IsValueType)
                return defaultValue;

            return func((T)o);
        }

        public static object MasterFunctionWrapper<TCast>(Func<object, TCast> func, object o) => func(o);
        
        public static object MasterFunctionWrapper<T>(Func<T, T> func, object o)
        {
            if (o == null && typeof(T).IsValueType)
                throw new InvalidOperationException($"Can not cast value to type of {typeof(T).Name} as it is null.");

            return func((T)o);
        }
    }
}