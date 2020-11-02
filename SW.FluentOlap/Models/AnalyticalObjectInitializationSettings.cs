using System;

namespace SW.FluentOlap.Models
{
    public class AnalyticalObjectInitializationSettings<T>
    {
        private const byte REFERENCELOOPSYSTEMLIMIT = 3;
        private byte _referenceLoopDepthLimit;
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
    }
}