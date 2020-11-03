using System;
using System.Linq.Expressions;

namespace SW.FluentOlap.Models
{
    public class PreInitializationAction<T>
    {
        public void Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var expression = (MemberExpression) propertyExpression.Body;
            string name = expression.Member.Name;
        }
    }
    public class AnalyticalObjectInitializationSettings<T>
    {
        public AnalyticalObjectInitializationSettings()
        {
            _referenceLoopDepthLimit = 1;
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

        public Action<PreInitializationAction<T>> PreInit
        {
            get;
            set;
        }
    }
}