using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class TypeMap : IDictionary<string, NodeProperties>
    {
        private readonly IDictionary<string, NodeProperties> _typeMap = new Dictionary<string, NodeProperties>();
        public NodeProperties this[string key] { get => _typeMap[key.ToLower()]; set => _typeMap[key.ToLower()] = value; }

        public ICollection<string> Keys => _typeMap.Keys;

        public ICollection<NodeProperties> Values => _typeMap.Values;

        public TypeMapDifferences GetDifferences(TypeMap other) => new TypeMapDifferences(this, other);

        public int Count => _typeMap.Count;

        public bool IsReadOnly => _typeMap.IsReadOnly;

        public void Add(string key, NodeProperties value)
        {
            _typeMap.Add(key, value);
        }

        public void Add(KeyValuePair<string, NodeProperties> item)
        {
            _typeMap.Add(item);
        }

        public void Clear()
        {
            _typeMap.Clear();
        }

        public bool Contains(KeyValuePair<string, NodeProperties> item) => _typeMap.Contains(item);

        public bool ContainsKey(string key) => _typeMap.ContainsKey(key.ToLower());


        public void CopyTo(KeyValuePair<string, NodeProperties>[] array, int arrayIndex)
        {
            _typeMap.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, NodeProperties>> GetEnumerator() => _typeMap.GetEnumerator();

        public bool Remove(string key) => _typeMap.Remove(key);

        public bool Remove(KeyValuePair<string, NodeProperties> item) => _typeMap.Remove(item);

        public bool TryGetValue(string key, out NodeProperties value) => _typeMap.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _typeMap.GetEnumerator();

    }
}
