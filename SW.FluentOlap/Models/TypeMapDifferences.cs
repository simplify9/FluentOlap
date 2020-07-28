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
                var difference =
                    new TypeMapDifference(entry.Key, 
                    DifferenceType.DataTypeChange, 
                    entry, 
                    second.FirstOrDefault(e => e.Key == entry.Key));

                if (second.Contains(entry))
                {
                    if(entry.Value != second[entry.Key])
                    {
                        typeMapDifferences.Add(difference);
                    }
                }
                else
                {
                    typeMapDifferences.Add(difference);
                }
            }
            foreach(var entry in second)
            {
                var difference =
                    new TypeMapDifference(entry.Key, 
                    DifferenceType.DataTypeChange, 
                    entry, 
                    first.FirstOrDefault(e => e.Key == entry.Key));

                if(!first.Contains(entry))
                    typeMapDifferences.Add(difference);
            }
        }

        public TypeMapDifferences()
        {
            typeMapDifferences = new List<TypeMapDifference>();
        }
    }
}
