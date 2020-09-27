namespace UtilityUnitTests.Models
{
    public class ParcelSelfReference
    {
        public string Id { get; set; }
        public ParcelSelfReference SelfReference { get; set; }
    }
}