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
        public string typeString { get; }
        public InternalType() {}
        public InternalType(string typeString)
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
        private const string SQLTYPEKEY = "sql_type";
        private const string NODENAMEKEY = "node_name";
        private const string UNIQUEKEY = "unique";
        private const string SERVICENAMEKEY = "service_name";
        public InternalType SqlType { get; set; }
        public bool Unique { get; set; }
        public string  NodeName { get; set; }
        public string ServiceName { get; set; }
        public override string ToString()
        {
            string stringified = $"{SQLTYPEKEY}={SqlType}&";
            stringified += $"{UNIQUEKEY}={Unique}&";
            stringified += $"{NODENAMEKEY}={NodeName?? "NULL"}&";
            stringified += $"{SERVICENAMEKEY}={ServiceName ?? "NULL"}";
            return stringified;
        }
        public static NodeProperties FromString(string s)
        {
            NodeProperties props = new NodeProperties();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach(string segment in s.Split('&'))
            {
                var segArr = segment.Split('=');
                pairs.Add(segArr[0], segArr[1]);
            }
            props.SqlType = new InternalType(pairs[SQLTYPEKEY]);
            props.NodeName = pairs[NODENAMEKEY] != "NULL"? pairs["node_name"] : null;
            props.ServiceName = pairs[SERVICENAMEKEY] != "NULL"? pairs[SERVICENAMEKEY] : null;
            props.Unique = bool.Parse(pairs[UNIQUEKEY]);
            return props;
        }
    }

}
