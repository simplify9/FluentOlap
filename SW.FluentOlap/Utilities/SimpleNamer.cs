using System.Collections.Generic;
using System.Linq;
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
        private bool TryGetMinimumUniqueKey(
            string[] hierarchy, 
            string propKey, 
            int index, 
            ICollection<string> keys,
            out string minimumKey)
        {
            minimumKey = string.Empty;
            for (int i = hierarchy.Length - 1; i >= index; i--)
                minimumKey = hierarchy[i] + SEPARATOR + minimumKey;

            minimumKey = (minimumKey + propKey).ToLower();

            if (!keys.Contains(minimumKey))
            {
                minimumKeyToHierarchy[minimumKey] = hierarchy;
                return true;
            }

            return false;
        }
        
        public string EnsureMinimumUniqueKey(
            string prefix, 
            string propKey, 
            TypeMap map,
            bool overwrite = false)
        {
            string[] hierarchy = prefix.Contains(SEPARATOR) ? prefix.Split(SEPARATOR) : new string[] {prefix};
            bool isUnique = TryGetMinimumUniqueKey(hierarchy, propKey, hierarchy.Length - 1, map.Keys, out string minimumKey);
            
            if (overwrite || isUnique) return minimumKey.ToLower();

            KeyValuePair<string, NodeProperties> existingTypeMap =
                map.FirstOrDefault(kv => kv.Key == minimumKey);
            map.Remove(existingTypeMap.Key);

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