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

    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IServiceInput
    {
        public string ChildKey { get; set; }
    }

    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IServiceOutput
    {
        public string ChildKey { get; set; }
        public PopulationResult PopulationResult { get; set; }
        public string RawOutput { get; }
    }

    /// <summary>
    /// Interface used for references of a service
    /// </summary>
    public interface IService
    {
        public string ServiceName { get; set; }
        public ServiceType Type { get; }
    }

    /// <summary>
    /// Parent abstract class of all services.
    /// </summary>
    /// <typeparam name="TIn">Input going to invocation</typeparam>
    /// <typeparam name="TOut">Return of invocation</typeparam>
    public abstract class Service<TIn, TOut> : IService
        where TIn : IServiceInput
        where TOut : IServiceOutput

    {
        protected Service(ServiceType type, string name)
        {
            ServiceName = name;
            Type = type;
        }

        /// <summary>
        /// How this service is used
        /// </summary>
        public abstract Func<TIn, Task<TOut>> InvokeAsync { get; }

        public string ServiceName { get; set; }
        public ServiceType Type { get; }
    }
}