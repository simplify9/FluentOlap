using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class MessageProperties
    {
        public string MessageName { get;  }
        public string KeyPath { get;  }
        public MessageProperties(string messageName, string keyPath)
        {
            this.MessageName = messageName;
            this.KeyPath = keyPath;
        }
    }
}
