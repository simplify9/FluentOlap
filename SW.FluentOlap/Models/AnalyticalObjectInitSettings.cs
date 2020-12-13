using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
            var expression = (MemberExpression) propertyExpression.Body;
            string ignoreKey = expression.Member.Name;
            expression = expression.Expression as MemberExpression;
            string prefix = string.Empty;
            while (expression != null)
            {
                string name = expression.Member.Name;
                expression = expression.Expression as MemberExpression;
                prefix = name + '_' + prefix;
            }

            prefix = typeof(T).Name + '_' + prefix;

            IgnoreList.Add(new KeyValuePair<string, string>(prefix.Substring(0, prefix.Length - 1), ignoreKey));
        }
        

    }
}