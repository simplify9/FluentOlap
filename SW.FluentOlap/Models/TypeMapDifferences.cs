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

                if (second.ContainsKey(entry.Key))
                {
                    if(entry.Value.SqlType.ToString() != second[entry.Key].SqlType.ToString())
                    {
                        var difference =
                            new TypeMapDifference(entry.Key, 
                            DifferenceType.DataTypeChange, 
                            entry, 
                            second.FirstOrDefault(e => e.Key == entry.Key));
                        typeMapDifferences.Add(difference);
                    }
                }
                else
                {
                        var difference =
                            new TypeMapDifference(entry.Key, 
                            DifferenceType.AddedColumn, 
                            entry, 
                            second.FirstOrDefault(e => e.Key == entry.Key));
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

                if(!first.ContainsKey(entry.Key))
                    typeMapDifferences.Add(difference);
            }
        }

        public TypeMapDifferences()
        {
            typeMapDifferences = new List<TypeMapDifference>();
        }
    }
}
