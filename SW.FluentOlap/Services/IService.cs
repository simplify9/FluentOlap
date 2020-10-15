using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SW.FluentOlap.Models
{
    public enum ServiceType
    {
        HttpCall,
        DatabaseCall
    }

    public interface IService
    {
        public ServiceType Type { get ; }
    }
    
    public abstract class Service<TIn, TOut> : IService
    {

        public Service(ServiceType type)
        {
            Type = type;
        }

        /// <summary>
        /// How this service is used
        /// </summary>
        public Func<TIn, Task<TOut>> InvokeAsync { get;}
        
        public ServiceType Type { get ; }

    }



    public class DatabaseService : IService<>
    {
        
        /// <summary>
        /// Select statement using SQL Parameters (eg @Id)
        /// </summary>
        public string Select { get; set; }

        public ServiceType Type => ServiceType.DatabaseCall;
    }
    
    
}
