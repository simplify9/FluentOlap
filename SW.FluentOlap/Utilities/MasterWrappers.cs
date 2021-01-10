using System;

namespace SW.FluentOlap.Utilities
{
    internal static class MasterWrappers
    {
        public static Func<object, object> MasterFunctionWrapper<T>(Func<T, T> func, object defaultValue)
        {
            return o =>
            {
                if (o == null && typeof(T).IsValueType)
                    return defaultValue;

                return func((T) o);
            };
        }

        public static Func<object, object> MasterFunctionWrapper<TCast>(Func<object, TCast> func) => o => func(o);

        public static Func<object, object> MasterFunctionWrapper<T>(Func<T, T> func)
        {
            return o =>
            {
                if (o == null && typeof(T).IsValueType)
                    throw new InvalidOperationException(
                        $"Can not cast value to type of {typeof(T).Name} as it is null.");

                return func((T) o);
            };
        }
    }
}