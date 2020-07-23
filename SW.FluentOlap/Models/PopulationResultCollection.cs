using SW.FluentOlap.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class PopulationResultCollection : ICollection<PopulationResult>
    {
        List<PopulationResult> inner = new List<PopulationResult>();
        public TypeMap OriginTypeMap { get; private set; }
        public string TargetTable { get; private set; }
        public int Count => inner.Count;

        public PopulationResult Sample => inner[0]?? null;
        public bool IsReadOnly => true;

        public void Add(PopulationResult item)
        {
            if(OriginTypeMap == null) OriginTypeMap = item.OriginTypeMap;
            if (TargetTable == null) TargetTable = item.TargetTable;
            if(TargetTable != item.TargetTable) throw new Exception("Target tables do not match.");
            if (Hashing.HashTypeMaps(item.OriginTypeMap) != Hashing.HashTypeMaps(OriginTypeMap)) throw new Exception("Type maps do not match.");

            inner.Add(item);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(PopulationResult item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(PopulationResult[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PopulationResult> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public bool Remove(PopulationResult item)
        {
            return inner.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }
}
