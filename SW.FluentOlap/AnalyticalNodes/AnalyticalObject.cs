using SW.FluentOlap.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using SW.FluentOlap.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SW.FluentOlap.Attributes;

namespace SW.FluentOlap.AnalyticalNode
{
    /// <summary>
    /// Object that keeps track of mappings for this object's children properties (and their children)
    /// </summary>
    /// <typeparam name="T">Type of object ot be analyzed</typeparam>
    public class AnalyticalObject<T> : IAnalyticalNode
    {
        public TypeMap TypeMap { get; protected set; }
        internal readonly SimpleNamer simpleNamer;
        public string ServiceName { get; set; }

        private const char SEPARATOR = '_';

        private const byte KeyLengthLimit = 64;
        private bool KeyLengthLimitSurpassed = false;

        internal IDictionary<string, IEnumerable<string>> minimumKeyToHierarchy { get; }


        private AnalyticalObjectInitSettings<T> initSettings;

        public MessageProperties MessageMap { get; set; }
        public string Name { get; set; }
        public Type AnalyzedType { get; set; }

        public Dictionary<string, NodeProperties> ExpandableChildren =>
            new Dictionary<string, NodeProperties>(TypeMap.Where(n => n.Value.ServiceName != null));


    public AnalyticalObject(Action<AnalyticalObjectInitSettings<T>> settings = null)
        {
            this.minimumKeyToHierarchy = new Dictionary<string, IEnumerable<string>>();
            initSettings = new AnalyticalObjectInitSettings<T>();

            settings?.Invoke(initSettings);

            AnalyzedType = typeof(T);
            Name = AnalyzedType.Name;
            TypeMap = new TypeMap(Name);
            
            simpleNamer = new SimpleNamer(SEPARATOR);

            InitTypeMap(AnalyzedType, AnalyzedType.Name, AnalyzedType.Name);
        }

        protected AnalyticalObject(TypeMap existing, IDictionary<string, IEnumerable<string>> minimumKeyToHierarchy)
        {
            this.minimumKeyToHierarchy = minimumKeyToHierarchy;
            simpleNamer = new SimpleNamer(SEPARATOR);
            TypeMap = existing;
        }


        private void InitTypeMap(Type typeToInit, string prefix, string preferredName, List<string> branchChain = null)
        {
            // Create the stem/branch if it does not exist
            branchChain ??= new List<string>();

            // Make sure occurrences of this type in the current branch do not exceed the limit
            int occurrences = branchChain.Count(v => v == typeToInit.FullName);
            if (occurrences > initSettings.ReferenceLoopDepthLimit) return;

            // Extend the branch
            branchChain.Add(typeToInit.FullName);

            // End of a branch.
            if (TypeUtils.TryGuessInternalType(typeToInit, out InternalType internalType)) // Primitive type
                PopulateTypeMaps(internalType, prefix, preferredName);

            else // This will have inner branches
            {
                // Keep track of the branch state as an origin point for the current complex type
                List<string> branchOrigin = branchChain.Select(v => v).ToList();

                // Concatenate parent
                if (prefix != preferredName && prefix != null) prefix = $"{prefix}_{preferredName}";

                // Recursively init each property
                foreach (PropertyInfo prop in typeToInit.GetProperties())
                {
                    // TODO: Create a more efficient ignore algorithm.
                    if (initSettings.IgnoreList.Contains(new KeyValuePair<string, string>(prefix, prop.Name))) continue;
                    if (prop.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                    // Passing the branch to extend it for the properties down the chain.
                    InitTypeMap(prop.PropertyType, prefix, prop.Name, branchChain);


                    // Effectively cutting off the branch once we are done with defining that property.
                    branchChain = branchOrigin.ToList();
                }
            }
        }

        /// <summary>
        /// Add a new map to the object's TypeMaps, by defining a type
        /// If it exists, it updates its type.
        /// </summary>
        /// <param name="type">SQL type</param>
        /// <param name="childName"></param>
        protected void PopulateTypeMaps(InternalType type, string prefix, string childName, bool overwrite = false)
        {
            string key = simpleNamer.EnsureMinimumUniqueKey(prefix, childName, TypeMap, overwrite);

            if (key.Length > KeyLengthLimit)
                KeyLengthLimitSurpassed = true;

            if (!TypeMap.ContainsKey(key)) TypeMap[key] = new NodeProperties(childName);


            TypeMap[key].InternalType = type;
        }

        public virtual AnalyticalObject<T> GetDirectParent() => null;

        protected void DeleteFromTypeMap(string childName, bool isPrimitive)
        {
            if (isPrimitive)
            {
                this.TypeMap.Remove(childName.ToLower());
            }
            else
            {
                foreach (var entry in this.TypeMap)
                {
                    string toRemove = childName.ToLower() + '_';
                    if (entry.Key.Contains(toRemove))
                        this.TypeMap.Remove(entry);
                }
            }
        }

        /// <summary>
        /// Define a service to pull information from
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="serviceUrl"></param>
        protected void PopulateServiceMaps(string serviceName, string prefix, string childName, string nodeName)
        {
            string key = $"{prefix}_{childName}";

            if (!TypeMap.ContainsKey(key)) TypeMap[key] = new NodeProperties(childName);

            TypeMap[key].ServiceName = serviceName.ToLower();
            TypeMap[key].NodeName = nodeName.ToLower();
        }

        public AnalyticalObject<T> Handles(string messageName, string keyPath)
        {
            this.MessageMap = new MessageProperties(messageName, keyPath);
            return this;
        }

        public AnalyticalObject<T> Handles<M>(Expression<Func<M, object>> propertyExpression)
        {
            string messageName = typeof(M).Name;
            var expression = (MemberExpression) propertyExpression.Body;
            string keyPath = expression.Member.Name;
            return this.Handles(messageName, keyPath);
        }

        public AnalyticalObject<T> GetFromService(string serviceName)
        {
            this.ServiceName = serviceName;
            return this;
        }

        private PopulationResult MergeIntoAggregate(PopulationResultCollection resultCollection)
        {
            IDictionary<string, object> merged = new Dictionary<string, object>();

            PopulationResult root = resultCollection.Dequeue();
            foreach ((string k, object v) in root)
            {
                string key = simpleNamer.EnsureMinimumUniqueKey(k, merged);
                merged[key] = v;
            }

            for (int _ = 0; _ < resultCollection.Count; ++_)
            {
                PopulationResult current = resultCollection.Dequeue();
                foreach ((string k, object v) in current)
                {
                    string key = simpleNamer.EnsureMinimumUniqueKey(k, merged);
                    merged[key] = v;
                }
            }


            return new PopulationResult(merged, TypeMap);
        }

        public async Task<PopulationResult> PopulateAsync<TInput>(TInput input)
            where TInput : IServiceInput
        {
            if (MessageMap == null)
                MessageMap = new MessageProperties("NONE", "Id");

            input.PrefixKey = Name;

            PopulationResultCollection rs = await DataCollector.CollectData(this, input);

            PopulationResult merged = MergeIntoAggregate(rs);
            return merged;
        }


        public AnalyticalChild<T, TProperty> Property<TProperty>(string propertyName, AnalyticalObject<TProperty> type)
        {
            var child = new AnalyticalChild<T, TProperty>(this, propertyName, type.AnalyzedType, this.TypeMap);
            PopulateTypeMaps(InternalType.NEVER, Name, propertyName);

            return child;
        }

        /// <summary>
        /// Pass in a property by LINQ expression so that it can be defined by modifying the resulting AnalyticalChild
        /// The AnalyticalChild constructor will try to guess the type.
        /// </summary>
        /// <typeparam name="TProperty">Type of property to be defined</typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="directParent">Direct parent, this should not be set by the user</param>
        /// <returns></returns>
        public AnalyticalChild<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            AnalyticalObject<T> directParent = null)
        {
            var expression = (MemberExpression) propertyExpression.Body;
            string name = expression.Member.Name;
            Type childType = propertyExpression.ReturnType;
            var child = new AnalyticalChild<T, TProperty>(directParent ?? this, name, childType, this.TypeMap);

            return child;
        }
    }
}