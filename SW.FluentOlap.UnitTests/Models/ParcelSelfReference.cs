using SW.FluentOlap.AnalyticalNode;

namespace UtilityUnitTests.Models
{
    public class DeeperSelfReference
    {
        public int Id { get; set; }
        public DeeperSelfReference SelfReference { get; set; }
    }
    public class ParcelSelfReference
    {
        public string Id { get; set; }
        public ParcelSelfReference SelfReference { get; set; }
        public DeeperSelfReference Deeper { get; set; }
    }

    public class WideParcelSelfReference : AnalyticalObject<ParcelSelfReference>
    {
        public WideParcelSelfReference()
        {
                
        }
    }
}