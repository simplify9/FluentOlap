using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class TypeMapDifferences : IEnumerable<TypeMapDifference>
    {
        private IList<TypeMapDifference> typeMapDifferences;
        private IEnumerable<DifferenceType> ignoredDifferenceTypes;

        public IEnumerator<TypeMapDifference> GetEnumerator()
        {
            return typeMapDifferences.GetEnumerator();
        }

        private void AddDifference(TypeMapDifference difference)
        {
            if (!ignoredDifferenceTypes.Contains(difference.DifferenceType))
                typeMapDifferences.Add(difference);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return typeMapDifferences.GetEnumerator();
        }

        public bool AreAllSimple() =>
            typeMapDifferences.All(tmd => tmd.DifferenceType != DifferenceType.DataTypeChange);

        public TypeMapDifferences(TypeMap first, TypeMap second, IEnumerable<DifferenceType> ignoredDifferenceTypes = null)
        {
            typeMapDifferences = new List<TypeMapDifference>();
            this.ignoredDifferenceTypes = ignoredDifferenceTypes?? new List<DifferenceType>();
            foreach (var entry in first)
            {
                if (second.ContainsKey(entry.Key))
                {
                    if (entry.Value.InternalType.ToString() != second[entry.Key].InternalType.ToString())
                    {
                        TypeMapDifference difference =
                            new TypeMapDifference(entry.Key,
                                DifferenceType.DataTypeChange,
                                second.FirstOrDefault(e => e.Key == entry.Key),
                                entry);
                        AddDifference(difference);
                    }
                }
                else
                {
                    TypeMapDifference difference =
                        new TypeMapDifference(entry.Key,
                            DifferenceType.RemovedColumn,
                            second.FirstOrDefault(e => e.Key == entry.Key),
                            entry);
                    AddDifference(difference);
                }
            }

            foreach (var entry in second)
            {
                var difference =
                    new TypeMapDifference(entry.Key,
                        DifferenceType.AddedColumn,
                        entry,
                        first.FirstOrDefault(e => e.Key == entry.Key));

                if (!first.ContainsKey(entry.Key))
                    AddDifference(difference);
            }

            var secondInOrder = second.AsEnumerable().ToArray();
            for (int i = 0; i < first.Count; ++i)
            {
                if (secondInOrder.Length < i + 1)
                    break;

                var atIndex = secondInOrder[i];
                foreach (var entry in first)
                {
                    bool keyInOrder = atIndex.Key == entry.Key;
                    if (!keyInOrder)
                    {
                        TypeMapDifference difference = new TypeMapDifference(entry.Key,
                            DifferenceType.ChangedColumnOrder,
                            entry,
                            atIndex
                        );
                        AddDifference(difference);
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