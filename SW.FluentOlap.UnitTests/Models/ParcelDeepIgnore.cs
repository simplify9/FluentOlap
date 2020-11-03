using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.Models;

namespace UtilityUnitTests.Models
{

    public class Complex
    {
        public string StringProp { get; set; }
        public int IntProp { get; set; }
    }

    public class ComplexObj2SameName
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
    }
    
    
    public class IgnoreTestMap
    {
        public ComplexObj2SameName Co2sm { get; set; }
        public Complex Complex { get; set; }
    }

    public class IgnoreMapAnalyzer : AnalyticalObject<IgnoreTestMap>
    {
        public IgnoreMapAnalyzer() : base(new AnalyticalObjectInitializationSettings<IgnoreTestMap>
        {
            PreInit = a =>
            {
                a.Ignore(p => p.Co2sm.Name);
                a.Ignore(p => p.Complex.IntProp);
            }
        })
        {
            Property(p => p.Co2sm).Ignore(c => c.Name);
            Property(p => p.Complex).Ignore(c => c.IntProp);
        }
    }
}