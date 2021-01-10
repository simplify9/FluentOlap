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
        /// <summary>
        /// Any transformation function can be stored here with a name,
        /// If AddTransformation is called providing an InternalType instance
        /// rather than a string then that transformation will
        /// be used as a default for that specific type if no other
        /// transformation is provided.
        /// </summary>
        public static TransformationsMasterList TransformationsMasterList { get; set; }

    }
}
