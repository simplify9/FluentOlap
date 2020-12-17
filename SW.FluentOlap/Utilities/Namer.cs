using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SW.FluentOlap.Models;

namespace SW.FluentOlap.Utilities
{
    internal class Namer
    {
        private readonly char SEPARATOR;
        private readonly Dictionary<string, string[]> minimumKeyToHierarchy;

        public Namer(char separator)
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
            (string key, string value) = GetPrefixAndKey(propertyExpression);
            return EnsureMinimumUniqueKey(key + SEPARATOR + value, map, overwrite);
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
        
        /// <summary>
        /// Ensures that the key is at its minimum length while remaining unique.
        /// </summary>
        /// <param name="prefix">The string segment without the final key</param>
        /// <param name="propKey">Final key</param>
        /// <param name="map">Dictionary in which the keys need to be unique.</param>
        /// <param name="overwrite">Whether or not to replace first existing member found with same minimum key</param>
        /// <typeparam name="TDicVal">Value property in the dictionary</typeparam>
        /// <returns></returns>
        
        public string EnsureMinimumUniqueKey<TDicVal>(string prefix, string propKey, IDictionary<string, TDicVal> map, bool overwrite = false)
        {
            string[] hierarchy = prefix.Contains(SEPARATOR) ? prefix.Split(SEPARATOR) : new string[] {prefix};
            bool isUnique = TryGetMinimumUniqueKey(hierarchy, propKey, hierarchy.Length - 1, map.Keys, out string minimumKey);
            
            if (overwrite || isUnique) return minimumKey.ToLower();

            // Removing data of member with same name.
            KeyValuePair<string, TDicVal> existingTypeMap =
                map.FirstOrDefault(kv => kv.Key == minimumKey);
            map.Remove(minimumKey);

            KeyValuePair<string, string[]> existingMinimumMap =
                minimumKeyToHierarchy.FirstOrDefault(kv => kv.Key == minimumKey);
            minimumKeyToHierarchy.Remove(existingMinimumMap.Key);

            string[] existingHierarchy = existingMinimumMap.Value.ToArray();

            bool opposingMore = existingHierarchy.Length > hierarchy.Length;
            //bool currentMore = existingHierarchy.Length > hierarchy.Length;

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