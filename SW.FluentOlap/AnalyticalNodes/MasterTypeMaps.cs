using SW.FluentOlap.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.AnalyticalNodes
{
    public class MasterTypeMaps : IEnumerable<KeyValuePair<string, TypeMap>>
    {
        public IDictionary<string, TypeMap> Maps { get;  }
        public IList<MessageProperties> MessageMaps { get; }
        public IDictionary<string, string> ServiceMaps { get; set; }
        public AnalyticalMetadata Metadata { get;  }
        public IEnumerator<KeyValuePair<string, TypeMap>> GetEnumerator()
        {
            return Maps.GetEnumerator();
        }
        public TypeMap this[string key] { get => this.Maps[key]; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Maps.GetEnumerator();
        }

        public MasterTypeMaps(AnalyticalMetadata metadata, IDictionary<string, TypeMap> maps, IList<MessageProperties> messageMaps)
        {
            Maps = maps;
            MessageMaps = messageMaps;
            Metadata = metadata;
        }
    }
}
