using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SW.FluentOlap.Utilities;

namespace SW.FluentOlap.Models
{
    
    public class AnalyticalObjectInitSettings<T>
    {
        
        internal IList<KeyValuePair<string, string>> IgnoreList { get; set; }
        
        public AnalyticalObjectInitSettings()
        {
            _referenceLoopDepthLimit = 1;
            IgnoreList = new List<KeyValuePair<string, string>>();
        }
        
        private const byte REFERENCELOOPSYSTEMLIMIT = 3;
        private byte _referenceLoopDepthLimit;
        /// <summary>
        /// How many times the same type can be referenced in a chain of properties.
        /// Maximum of 3, default of 1. 0 would disable self referencing.
        /// </summary>
        public byte ReferenceLoopDepthLimit
        {
            get => _referenceLoopDepthLimit;
            set
            {
                if (value < REFERENCELOOPSYSTEMLIMIT)
                    _referenceLoopDepthLimit = value;
                else
                    throw new InvalidOperationException($"Maximum of {REFERENCELOOPSYSTEMLIMIT} ");
            }
        }
        
        public void Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {

            IgnoreList.Add(Namer.GetPrefixAndKey(propertyExpression));
        }
        

    }
}