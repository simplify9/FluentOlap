using System;
using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.Models;

namespace UtilityUnitTests.Models
{
    public class Parcel1Level
    {
        public string Id { get; set; }
        public int Location { get; set; }
        public int? Count { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class Parcel1LevelAnalyzer : AnalyticalObject<Parcel1Level>
    {
        public Parcel1LevelAnalyzer()
        {
            Property(p => p.Id);
            Property(p => p.Location);
            Property(p => p.Count);
            Property(p => p.DateTime);
        }
        
    }
    public class DefaultInitParcel1LevelAnalyzer : AnalyticalObject<Parcel1Level>
    {
        public DefaultInitParcel1LevelAnalyzer()
        {

        }
    }
}
