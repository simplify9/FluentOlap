using System.Collections;
using System.Collections.Generic;

namespace SW.FluentOlap.Models
{
    public class ServiceDefinitions : IDictionary<string, Service>
    {
        private readonly IDictionary<string, Service> _services = new Dictionary<string, Service>();
        public Service this[string key] { get => _services[key.ToLower()]; set => _services[key.ToLower()] = value; }

        public ICollection<string> Keys => _services.Keys;

        public ICollection<Service> Values => _services.Values;

        public int Count => _services.Count;

        public bool IsReadOnly => _services.IsReadOnly;

        public void Add(string key, IService value)
        {
            _services.Add(key.ToLower(), value);
        }


        public void Add(KeyValuePair<string, IService> item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public bool Contains(KeyValuePair<string, IService> item)
        {
            return _services.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _services.ContainsKey(key.ToLower());
        }

        public void CopyTo(KeyValuePair<string, IService>[] array, int arrayIndex)
        {
            _services.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, IService>> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _services.Remove(key.ToLower());
        }

        public bool Remove(KeyValuePair<string, IService> item)
        {
            return _services.Remove(item);
        }

        public bool TryGetValue(string key, out IService value)
        {
            return _services.TryGetValue(key.ToLower(), out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _services.GetEnumerator();
        }
    }
}
