using SW.FluentOlap.AnalyticalNode;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityUnitTests.Models
{
    public class Shipper
    {
        public string Name { get; set; }
        public string OriginCountry { get; set; }
    }
    public class Supplier
    {
        public string Name { get; set; }
        public string DestinationCountry { get; set; }
    }
    public class Parcel2Level
    {
        public string Id { get; set; }
        public int Location { get; set; }
        public int Count { get; set; }

        public Shipper Shipper { get; set; }
        public Supplier Supplier { get; set; }

    }

    public class Parcel2LevelAnalyzer : AnalyticalObject<Parcel2Level>
    {
        public Parcel2LevelAnalyzer()
        {
            Property(p => p.Id);
            Property(p => p.Location);
            Property(p => p.Count);
            Property(p => p.Shipper).Property(s => s.Name);
            Property(p => p.Shipper).Property(s => s.OriginCountry);
            Property(p => p.Supplier).Property(s => s.Name);
            Property(p => p.Supplier).Property(s => s.DestinationCountry);
        }
    }
}
