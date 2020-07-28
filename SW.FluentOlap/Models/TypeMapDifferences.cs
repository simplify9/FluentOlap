using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class TypeMapDifferences : IEnumerable<TypeMapDifference>
    {

        private IList<TypeMapDifference> typeMapDifferences;
        public IEnumerator<TypeMapDifference> GetEnumerator()
        {
            return typeMapDifferences.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return typeMapDifferences.GetEnumerator();
        }

        public bool AreAllSimple() => typeMapDifferences.All(tmd => tmd.DifferenceType != DifferenceType.DataTypeChange);

        public TypeMapDifferences(TypeMap first, TypeMap second)
        {
            typeMapDifferences = new List<TypeMapDifference>();
            foreach(var entry in first)
            {
                if (second.Contains(entry))
                {
                    if(entry.Value != second[entry.Key])
                    {
                        typeMapDifferences.Add(new TypeMapDifference(entry.Key, DifferenceType.DataTypeChange));
                    }
                }
                else
                {
                    typeMapDifferences.Add(new TypeMapDifference(entry.Key, DifferenceType.RemovedColumn));
                }
            }
            foreach(var entry in second)
            {
                if(!first.Contains(entry))
                    typeMapDifferences.Add(new TypeMapDifference(entry.Key, DifferenceType.AddedColumn));
            }
        }

        public TypeMapDifferences()
        {
            typeMapDifferences = new List<TypeMapDifference>();
        }
    }
}
