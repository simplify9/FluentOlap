using SW.FluentOlap.AnalyticalNode;

namespace UtilityUnitTests.Models
{
    public class DeeperSelfReference
    {
        public int Number { get; set; }
        public DeeperSelfReference DeeperSelfRef{ get; set; }
        //public ParcelSelfReference ParcelSelfRef{ get; set; }
    }
    public class ParcelSelfReference
    {
        public int Id { get; set; }
        public ParcelSelfReference SelfRef { get; set; }
        public DeeperSelfReference Deeper  { get; set; }
    }


    public class WideParcelSelfReference : AnalyticalObject<ParcelSelfReference>
    {
        public WideParcelSelfReference()
        {
                
        }
    }
}