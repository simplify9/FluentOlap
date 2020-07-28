using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public enum DifferenceType
    {
        DataTypeChange,
        AddedColumn,
        RemovedColumn
    }
    public class TypeMapDifference
    {
        private string columnKey;
        public string ColumnKey { get => columnKey.ToLower(); set => columnKey = value.ToLower(); }
        public DifferenceType DifferenceType { get; set; }

        public TypeMapDifference() {}
        public TypeMapDifference(string columnKey, DifferenceType differenceType)
        {
            ColumnKey = columnKey;
            DifferenceType = differenceType;
        }

    }
}
