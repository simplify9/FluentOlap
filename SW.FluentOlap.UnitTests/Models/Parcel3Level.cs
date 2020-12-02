using SW.FluentOlap.AnalyticalNode;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityUnitTests.Models
{
    public class Parcel3Level
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string ReferenceToParcel2Level { get; set; }
    }

    public class Parcel3LevelAnalyzer : AnalyticalObject<Parcel3Level>
    {
        public Parcel3LevelAnalyzer()
        {
            Property(p => p.Id);
            Property(p => p.Reference);
        }
    }
}
