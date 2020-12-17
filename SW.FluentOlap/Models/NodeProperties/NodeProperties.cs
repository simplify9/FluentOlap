using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

namespace SW.FluentOlap.Models
{
    internal class NodePropKeys
    {
    }
    public class InternalType
    {
        private sealed class TypeStringEqualityComparer : IEqualityComparer<InternalType>
        {
            public bool Equals(InternalType x, InternalType y)
            {
                if (x.GetType() != y.GetType()) return false;
                return x.typeString == y.typeString;
            }

            public int GetHashCode(InternalType obj)
            {
                return (obj.typeString != null ? obj.typeString.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<InternalType> TypeStringComparer { get; } = new TypeStringEqualityComparer();

        public string typeString { get; }
        public InternalType() {}
        public InternalType(string typeString)
        {
            this.typeString = typeString;
        }
        public static InternalType STRING => new InternalType("STRING");
        public static InternalType INTEGER => new InternalType("INTEGER");

        public static InternalType NEVER => new InternalType("NEVER");

        public static InternalType FLOAT => new InternalType("FLOAT");

        public static InternalType BOOLEAN => new InternalType("BOOLEAN");

        public static InternalType DATETIME => new InternalType("DATETIME");

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;

            InternalType other = (InternalType)obj;
            
            return typeString == other.typeString;
        }

        public override int GetHashCode()
        {
            return (typeString != null ? typeString.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return typeString;
        }
    }
    public class NodeProperties
    {
        private const string SQLTYPEKEY = "internal_type";
        private const string NODENAMEKEY = "node_name";
        private const string UNIQUEKEY = "unique";
        private const string SERVICENAMEKEY = "service_name";
        public InternalType InternalType { get; set; }
        public bool Unique { get; set; }
        public string  NodeName { get; set; }
        public string Name { get; }
        public string ServiceName { get; set; }
        public Func<object, object> Transformation { get; set; }

        public NodeProperties(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            string stringified = $"{SQLTYPEKEY}={InternalType}&";
            stringified += $"{UNIQUEKEY}={Unique}&";
            stringified += $"{NODENAMEKEY}={NodeName?? "NULL"}&";
            stringified += $"{SERVICENAMEKEY}={ServiceName ?? "NULL"}";
            return stringified;
        }
        public static NodeProperties FromString(string s)
        {
            //TODO: Get name
            NodeProperties props = new NodeProperties("");
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach(string segment in s.Split('&'))
            {
                var segArr = segment.Split('=');
                pairs.Add(segArr[0], segArr[1]);
            }
            props.InternalType = new InternalType(pairs[SQLTYPEKEY]);
            props.NodeName = pairs[NODENAMEKEY] != "NULL"? pairs["node_name"] : null;
            props.ServiceName = pairs[SERVICENAMEKEY] != "NULL"? pairs[SERVICENAMEKEY] : null;
            props.Unique = bool.Parse(pairs[UNIQUEKEY]);
            return props;
        }
    }

}
