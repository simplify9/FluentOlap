using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class InternalType
    {
        public string typeString { get; }
        public InternalType() {}
        private InternalType(string typeString)
        {
            this.typeString = typeString;
        }
        public static InternalType STRING {
            get => new InternalType("STRING");
        }
        public static InternalType INTEGER {
            get => new InternalType("INTEGER");
        }
        public static InternalType FLOAT {
            get => new InternalType("FLOAT");
        }
        public static InternalType BOOLEAN
        {
            get => new InternalType("BOOLEAN");
        }
        public static InternalType DATETIME
        {
            get => new InternalType("DATETIME");
        }

        public override bool Equals(object obj)
        {
            InternalType t = (InternalType)obj;
            if (t.typeString == this.typeString) return true;
            else return false;
        }

        public override string ToString()
        {
            return typeString;
        }
    }
    public class NodeProperties
    {
        public InternalType SqlType { get; set; }
        public string  NodeName { get; set; }
        public bool Unique { get; set; }
        public string ServiceName { get; set; }
        public override string ToString()
        {
            string stringified = $"sqlType={SqlType}&";
            stringified += $"unique={Unique}";
            return stringified;
        }
    }

}
