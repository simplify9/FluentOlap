using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.AnalyticalNode
{
    public interface IAnalyticalNode
    {
        TypeMap TypeMap { get; }
        string Name { get; set; }
        string ServiceName { get; set; }
        Type AnalyzedType { get; set; }

        MessageProperties MessageMap { get; }
    }
}
