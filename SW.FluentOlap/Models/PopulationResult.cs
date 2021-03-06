﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SW.FluentOlap.Utilities;

namespace SW.FluentOlap.Models
{
    /// <summary>
    /// Dictionary containing the results of a service
    /// </summary>
    public class PopulationResult : IReadOnlyDictionary<string, object>
    {
        public string Raw { get; }
        private readonly IDictionary<string, object> inner;
        public object this[string key] => inner[key];
        public IEnumerable<string> Keys => inner.Keys;
        public IEnumerable<object> Values => inner.Values;
        public int Count => inner.Count;
        public bool ContainsKey(string key) => inner.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => inner.GetEnumerator();
        public bool TryGetValue(string key, out object value) => inner.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
        public TypeMap OriginTypeMap { get; }

        /// <summary>
        /// Used for the final population result.
        /// </summary>
        /// <param name="flattened"></param>
        /// <param name="typeMap"></param>
        public PopulationResult(IDictionary<string, object> flattened, TypeMap typeMap)
        {
            OriginTypeMap = typeMap;
            var filteredResult
                = new Dictionary<string, object>(
                    flattened.Where(v => OriginTypeMap.ContainsKey(v.Key))
                        .ToDictionary(kv => kv.Key, kv => kv.Value));
            ;

            var neededTransformation =
                typeMap.Where(pair => pair.Value.Transformation != null ||
                              FluentOlapConfiguration.TransformationsMasterList.ContainsKey(pair.Value
                              .InternalType.typeString)
                ).Select(i => i.Key).ToList();

            foreach (string key in neededTransformation)
            {
                NodeProperties props = typeMap[key];
                
                // Default to the master list.
                props.Transformation ??= FluentOlapConfiguration.TransformationsMasterList[props.InternalType];
                
                if (filteredResult.TryGetValue(key, out object current))
                {
                    filteredResult[key] = props.Transformation(current);
                }
                else
                {
                    filteredResult[key] = props.Transformation(null);
                }
            }

            inner = filteredResult;
        }

        /// <summary>
        /// Automatically de-normalize json.
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="prefix"></param>
        public PopulationResult(string raw, string prefix = "")
        {
            Raw = raw;
            inner = JsonHelper.DeserializeAndFlatten(raw, prefix);
        }
    }
}