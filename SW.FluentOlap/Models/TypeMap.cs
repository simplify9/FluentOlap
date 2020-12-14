using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TypeMapNode = System.Collections.Generic.KeyValuePair<string, SW.FluentOlap.Models.NodeProperties>;

namespace SW.FluentOlap.Models
{
    public class TypeMap : IDictionary<string, NodeProperties>
    {
        public TypeMap() { }
        internal TypeMap(string name)
        {
            Name = name;
        }
        private readonly IDictionary<string, NodeProperties> _typeMap = new Dictionary<string, NodeProperties>();
        public NodeProperties this[string key] { get => _typeMap[key.ToLower()]; set => _typeMap[key.ToLower()] = value; }

        public ICollection<string> Keys => _typeMap.Keys;

        public ICollection<NodeProperties> Values => _typeMap.Values;

        public string Name { get; }
        public TypeMapDifferences GetDifferences(TypeMap other) => new TypeMapDifferences(this, other);

        public int Count => _typeMap.Count;

        public bool IsReadOnly => _typeMap.IsReadOnly;

        public void Add(string key, NodeProperties value)
        {
            _typeMap.Add(key, value);
        }

        public void Add(TypeMapNode item)
        {
            _typeMap.Add(item);
        }

        public void Clear()
        {
            _typeMap.Clear();
        }

        public bool Contains(TypeMapNode item) => _typeMap.Contains(item);

        public bool ContainsKey(string key) => _typeMap.ContainsKey(key.ToLower());

        public string EncodeToBase64()
        {
            byte[] mapBytes = Encoding.ASCII.GetBytes(this.ToString());
            string b64 =  Convert.ToBase64String(mapBytes);
            return b64;
        }

        public static TypeMap DecodeFromBase64(string base64)
        {
            byte[] mapBytes = Convert.FromBase64String(base64);
            string typeMapString = Encoding.ASCII.GetString(mapBytes);
            return TypeMap.FromString(typeMapString);
        }

        public void CopyTo(TypeMapNode[] array, int arrayIndex)
        {
            _typeMap.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TypeMapNode> GetEnumerator() => _typeMap.GetEnumerator();

        public bool Remove(string key) => _typeMap.Remove(key);

        public bool Remove(TypeMapNode item) => _typeMap.Remove(item);

        public bool TryGetValue(string key, out NodeProperties value) => _typeMap.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _typeMap.GetEnumerator();

        public static TypeMap FromString(string s)
        {
            TypeMap map = new TypeMap();
            foreach(string segment in s.Split(','))
            {
                if (segment == "") continue;
                string[] segArr = segment.Split(':');
                map.Add(segArr[0], NodeProperties.FromString(segArr[1]));
            }
            return map;
        }
        public override string ToString()
        {
            string typeMapsString = string.Empty;
            foreach(TypeMapNode map in this)
            {
                string mapString = $"{map.Key}:{map.Value},";
                typeMapsString += mapString;
            }
            return typeMapsString;

        }

        public static implicit operator Dictionary<string, object>(TypeMap map)
        {
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            
            foreach ((string key, NodeProperties value) in map)
                tmp[key] = value;

            return tmp;
        }
        

    }
}
