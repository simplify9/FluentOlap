using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class PopulationResult : IReadOnlyDictionary<string, object>
    {
        private readonly IDictionary<string, object> inner = new Dictionary<string, object>();
        public TypeMap OriginTypeMap { get; }
        public string TargetTable { get; set; }
        public object this[string key] => inner[key];
        public IEnumerable<string> Keys => inner.Keys;
        public IEnumerable<object> Values => inner.Values;
        public int Count => inner.Count;
        public bool ContainsKey(string key) => inner.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => inner.GetEnumerator();
        public bool TryGetValue(string key, out object value) => inner.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
        public PopulationResult(TypeMap typeMap, IDictionary<string, object> rs, string targetTable = null)
        {
            this.OriginTypeMap = typeMap;
            foreach(var entry in typeMap)
            {
                //TODO Validate against types
                if (rs.ContainsKey(entry.Key)) inner.Add(entry.Key, rs[entry.Key]);
                else inner.Add(entry.Key, null);
            }
        }
    }
}
