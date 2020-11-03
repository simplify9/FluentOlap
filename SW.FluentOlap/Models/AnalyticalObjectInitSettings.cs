using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SW.FluentOlap.Models
{
    
    public class AnalyticalObjectInitSettings<T>
    {
        
        internal IList<string> IgnoreList { get; set; }
        
        public AnalyticalObjectInitSettings()
        {
            _referenceLoopDepthLimit = 1;
            IgnoreList = new List<string>();
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
            var expression = (MemberExpression) propertyExpression.Body;
            string ignoreKey = string.Empty;
            while (expression != null)
            {
                string name = expression.Member.Name;
                expression = expression.Expression as MemberExpression;
                ignoreKey = name + '_' + ignoreKey;
            }

            ignoreKey = typeof(T).Name + '_' + ignoreKey;

            IgnoreList.Add(ignoreKey.Substring(0, ignoreKey.Length - 1).ToLower());

        }
        

    }
}