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
                    if(entry.Value.InternalType.ToString() != second[entry.Key].InternalType.ToString())
                    {
                        var difference =
                            new TypeMapDifference(entry.Key,
                            DifferenceType.DataTypeChange,
                            second.FirstOrDefault(e => e.Key == entry.Key),
                            entry);
                        typeMapDifferences.Add(difference);
                    }
                }
                else
                {
                    var difference =
                        new TypeMapDifference(entry.Key,
                        DifferenceType.RemovedColumn,
                        second.FirstOrDefault(e => e.Key == entry.Key),
                        entry); 
                    typeMapDifferences.Add(difference);
                }
            }
            foreach(var entry in second)
            {
                var difference =
                    new TypeMapDifference(entry.Key, 
                    DifferenceType.AddedColumn, 
                    entry, 
                    first.FirstOrDefault(e => e.Key == entry.Key));

                if(!first.ContainsKey(entry.Key))
                    typeMapDifferences.Add(difference);
            }

            var secondInOrder = second.AsEnumerable().ToArray();
            for (int i = 0; i < first.Count; ++i)
            {
                var atIndex = secondInOrder[i];
                foreach (var entry in first)
                {
                    bool keyInOrder = atIndex.Key == entry.Key;
                    if (!keyInOrder)
                    {
                        typeMapDifferences.Add(new TypeMapDifference(entry.Key,
                            DifferenceType.ChangedColumnOrder,
                            entry,
                            atIndex
                            ));
                    }
                }
            }
        }

        public TypeMapDifferences()
        {
            typeMapDifferences = new List<TypeMapDifference>();
        }
    }
}
