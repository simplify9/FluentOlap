using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SW.FluentOlap.Models;

namespace SW.FluentOlap.Utilities
{
    internal class SimpleNamer
    {
        private readonly char SEPARATOR;
        private readonly Dictionary<string, string[]> minimumKeyToHierarchy;

        public SimpleNamer(char separator)
        {
            SEPARATOR = separator;
            minimumKeyToHierarchy = new Dictionary<string, string[]>();
        }
        private bool TryGetMinimumUniqueKey(string[] hierarchy, string propKey, int index, ICollection<string> keys, out string minimumKey)
        {
            minimumKey = string.Empty;
            for (int i = hierarchy.Length - 1; i >= index; i--)
                minimumKey = hierarchy[i] + SEPARATOR + minimumKey;

            minimumKey = (minimumKey + propKey).ToLower();
            
            if (keys.Contains(minimumKey)) return false;
            
            minimumKeyToHierarchy[minimumKey] = hierarchy;
            return true;

        }

        public string EnsureMinimumUniqueKey<TDicVal, T, TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            IDictionary<string, TDicVal> map,
            bool overwrite = false)
        {
            var kv = GetPrefixAndKey(propertyExpression);
            return EnsureMinimumUniqueKey(kv.Key + SEPARATOR + kv.Value, map, overwrite);
        }

        public string EnsureMinimumUniqueKey<TDicVal>(string fullKey, IDictionary<string, TDicVal> map,
            bool overwrite = false)
        {
            string[] split = fullKey.Split(SEPARATOR);
            string prefix = string.Join(SEPARATOR, split.Take(split.Length - 1));
            string propKey = split.TakeLast(1).First();
            return EnsureMinimumUniqueKey(prefix, propKey, map, overwrite);
        }


        public static KeyValuePair<string, string> GetPrefixAndKey<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            
            var expression = (MemberExpression) propertyExpression.Body;
            string key = expression.Member.Name;
            expression = expression.Expression as MemberExpression;
            string prefix = string.Empty;
            while (expression != null)
            {
                string name = expression.Member.Name;
                expression = expression.Expression as MemberExpression;
                prefix = name + '_' + prefix;
            }

            prefix = typeof(T).Name + '_' + prefix;
            return new KeyValuePair<string, string>(prefix.Substring(0, prefix.Length - 1), key);
        }
        
        
        public string EnsureMinimumUniqueKey<TDicVal>(string prefix, string propKey, IDictionary<string, TDicVal> map, bool overwrite = false)
        {
            string[] hierarchy = prefix.Contains(SEPARATOR) ? prefix.Split(SEPARATOR) : new string[] {prefix};
            bool isUnique = TryGetMinimumUniqueKey(hierarchy, propKey, hierarchy.Length - 1, map.Keys, out string minimumKey);
            
            if (overwrite || isUnique) return minimumKey.ToLower();

            KeyValuePair<string, TDicVal> existingTypeMap =
                map.FirstOrDefault(kv => kv.Key == minimumKey);
            map.Remove(minimumKey);

            KeyValuePair<string, string[]> existingMinimumMap =
                minimumKeyToHierarchy.FirstOrDefault(kv => kv.Key == minimumKey);
            minimumKeyToHierarchy.Remove(existingMinimumMap.Key);

            bool opposingMore = existingMinimumMap.Value.ToArray().Length > hierarchy.Length;

            for (int i = hierarchy.Length - 1; i >= 0; --i)
            {
                bool current =
                    TryGetMinimumUniqueKey(hierarchy, propKey, i, map.Keys, out minimumKey);

                bool existing =
                    TryGetMinimumUniqueKey(existingMinimumMap.Value.ToArray(), propKey, i, map.Keys, out string existingNewKey);


                if (current && existing && existingNewKey != minimumKey)
                {
                    map[existingNewKey] = existingTypeMap.Value;
                    return minimumKey.ToLower();
                }
            }

            if (opposingMore)
            {
                // This will fail for certain edge cases
                bool existing = 
                    TryGetMinimumUniqueKey(existingMinimumMap.Value.ToArray(), propKey, hierarchy.Length, map.Keys, out string existingNewKey);
                map[existingNewKey] = existingTypeMap.Value;
                return minimumKey.ToLower();
            }
            else
            {
                // Either the key has been found, or someone is overwriting a property.
                return minimumKey.ToLower();
            }
        }

    }
}