using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class PopulationContext<T>
    {
        public T Input { get; set; }
        public PopulationContext(T input)
        {
            Input = input;
        }
        
    }
}
