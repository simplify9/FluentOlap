using SW.FluentOlap.AnalyticalNode;

namespace UtilityUnitTests.Models
{
    public class DeeperSelfReference
    {
        public int Number { get; set; }
        public DeeperSelfReference DeeperSelfRef{ get; set; }
    }
    public class ParcelSelfReference
    {
        public int Id { get; set; }
        public ParcelSelfReference SelfRef { get; set; }
        public DeeperSelfReference Deeper  { get; set; }
    }
    
    public class DeeperSelfReference2
    {
        public int Number { get; set; }
        public DeeperSelfReference2 DeeperSelfRef{ get; set; }
        public ParcelSelfReference2 ParcelSelfRef { get; set; }
    }
    public class ParcelSelfReference2
    {
        public int Id { get; set; }
        public ParcelSelfReference2 SelfRef { get; set; }
        public DeeperSelfReference2 Deeper  { get; set; }
    }


    public class WideParcelSelfReferenceDeep : AnalyticalObject<ParcelSelfReference2> { }
    public class WideParcelSelfReference : AnalyticalObject<ParcelSelfReference> { }
}