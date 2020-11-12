using SW.FluentOlap.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class PopulationResultCollection : Queue<PopulationResult>
    {
        Queue<PopulationResult> inner = new Queue<PopulationResult>();
        public TypeMap OriginTypeMap { get; private set; }
        public new int Count => inner.Count;

        public PopulationResult Sample => inner.Peek()?? null;
        public bool IsReadOnly => true;

        public new PopulationResult Dequeue()
        {
            return inner.Dequeue();
        }

        public void Add(PopulationResult item)
        {
            inner.Enqueue(item);
        }

        public new void Clear()
        {
            inner.Clear();
        }

        public new bool Contains(PopulationResult item)
        {
            return inner.Contains(item);
        }

        public new void CopyTo(PopulationResult[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public new IEnumerator<PopulationResult> GetEnumerator()
        {
            return inner.GetEnumerator();
        }


    }
}
