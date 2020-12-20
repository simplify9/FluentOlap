using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SW.FluentOlap.Utilities;

namespace SW.FluentOlap.Models
{
    public class TransformationsMasterList :  IReadOnlyDictionary<string, Func<object, object>>
    {
        public Dictionary<string, Func<object, object>> _transformations;

        public TransformationsMasterList()
        {
            _transformations = new Dictionary<string, Func<object, object>>();
        }
        public IEnumerator<KeyValuePair<string, Func<object, object>>> GetEnumerator()
        {
            return _transformations.GetEnumerator();
        }

        public void AddTransformation<T>(string key, Func<T, T> transformation)
        {
            _transformations[key] = o => MasterWrappers.MasterFunctionWrapper(transformation, o);
        }
        public void AddTransformation<T,TCast>(string key, Func<object, TCast> transformation)
        {
            _transformations[key] = o => MasterWrappers.MasterFunctionWrapper(transformation, o);
        }
        public void AddTransformation<T,TCast>(string key, Func<T, T> transformation, object defaultValue)
        {
            _transformations[key] = o => MasterWrappers.MasterFunctionWrapper(transformation, o, defaultValue);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _transformations.Count;

        public bool ContainsKey(string key)
        {
            return _transformations.ContainsKey(key);
        }

        public bool TryGetValue(string key, out Func<object, object> value)
        {
            return _transformations.TryGetValue(key, out value);
        }

        public Func<object, object> this[string key] => throw new NotImplementedException();

        public IEnumerable<string> Keys => _transformations.Keys;
        public IEnumerable<Func<object, object>> Values => _transformations.Values;
    }
}