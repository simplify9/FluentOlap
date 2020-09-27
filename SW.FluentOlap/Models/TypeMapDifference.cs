using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public enum DifferenceType
    {
        DataTypeChange,
        AddedColumn,
        RemovedColumn,
        ChangedColumnOrder
    }
    public class TypeMapDifference
    {
        private readonly string columnKey;
        public string ColumnKey { get => columnKey.ToLower(); }
        public DifferenceType DifferenceType { get; }
        public KeyValuePair<string, NodeProperties> ModifiedColumn { get; }
        public KeyValuePair<string, NodeProperties> OriginalColumn { get; }


        public TypeMapDifference() {}
        public TypeMapDifference(string columnKey, DifferenceType differenceType,
                                 KeyValuePair<string, NodeProperties> modified,
                                 KeyValuePair<string, NodeProperties> original)
        {
            this.columnKey = columnKey.ToLower();
            DifferenceType = differenceType;
            ModifiedColumn = modified;
            OriginalColumn = original;
        }

    }
}
