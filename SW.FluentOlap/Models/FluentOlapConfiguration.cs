using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Models
{
    public static class FluentOlapConfiguration
    {
        static FluentOlapConfiguration()
        {
            TransformationsMasterList = new TransformationsMasterList();
        }
        public static ServiceDefinitions ServiceDefinitions { get; set; }
        public static TransformationsMasterList TransformationsMasterList { get; set; }

    }
}
