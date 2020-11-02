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
        public static Type OriginType { get; private set; }
        private static TypeMap FinalTypeMap { get; set; }
        public TypeMap TypeMap { get; protected set; }
        public string ServiceName { get; set; }


        public static byte SelfReferencingLimit
        {
            get => 1;
        }

        public MessageProperties MessageMap { get; set; }
        public string Name { get; set; }
        private IEnumerable<IEnumerable<Type>> TypeChains { get; set; }
        public Type AnalyzedType { get; set; }

        public Dictionary<string, NodeProperties> ExpandableChildren =>
            new Dictionary<string, NodeProperties>(TypeMap.Where(n => n.Value.ServiceName != null));

        public AnalyticalObject()
        {
            TypeMap = new TypeMap();
            AnalyzedType = typeof(T);
            Name = AnalyzedType.Name;
            InitTypeMap();
        }

        public AnalyticalObject(TypeMap existing)
        {
            TypeMap = existing;
        }

        private void InitTypeMap(Type typeToInit = null, string prefix = null, string preferredName = null,
            string directParentName = null, List<string> branchChain = null)
        {
            typeToInit ??= this.AnalyzedType;
            prefix ??= this.AnalyzedType.Name;
            preferredName ??= typeToInit.Name;
            directParentName ??= preferredName;
            

            // If there is a reference to a type that exists previously in the chain
            // Make sure the reference count is not more than the predefined limit
            // If it is, skip the definition.
            // Else proceed as normal
            branchChain ??= new List<string>();
            if (branchChain.Contains(typeToInit.FullName)) // A reference to a (grand)parent of the same type
            {
                int occurenceCount = branchChain.Count(v => v == typeToInit.FullName);
                if (occurenceCount > SelfReferencingLimit) return;
            }

            branchChain.Add(typeToInit.FullName);

            List<string> branchOrigin = branchChain.Select(v => v).ToList();
            if (TypeUtils.TryGuessInternalType(typeToInit, out InternalType internalType))
            {
                PopulateTypeMaps(internalType, $"{prefix}_{preferredName}");
            }
            else
            {
                if (prefix != directParentName) prefix = $"{prefix}_{directParentName}";
                foreach (PropertyInfo prop in typeToInit.GetProperties())
                {
                    if (prop.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                    InitTypeMap(prop.PropertyType, $"{prefix}", prop.Name, null, branchChain);
                    //Clone
                    branchChain = branchOrigin.Select(v => v).ToList();
                }
            }
        }

        /// <summary>
        /// Add a new map to the object's TypeMaps, by defining a type
        /// If it exists, it updates its type.
        /// </summary>
        /// <param name="type">SQL type</param>
        /// <param name="childName"></param>
        protected void PopulateTypeMaps(InternalType type, string childName)
        {
            string key = childName;
            if (!this.TypeMap.ContainsKey(key))
            {
                this.TypeMap[key] = new NodeProperties
                {
                    InternalType = type
                };
            }
            else
            {
                this.TypeMap[key].InternalType = type;
            }
        }

        public virtual AnalyticalObject<T> GetDirectParent()
        {
            return null;
        }

        /// <summary>
        /// Add a new map  by setting it to unique
        /// Or update the UNIQUE property if it exists
        /// </summary>
        /// <param name="isUnique"></param>
        /// <param name="childName"></param>
        protected virtual void IsUnique(bool isUnique, string childName)
        {
            string key = childName;
            if (!this.TypeMap.ContainsKey(key))
            {
                this.TypeMap[key] = new NodeProperties
                {
                    Unique = isUnique
                };
            }
            else
            {
                this.TypeMap[key].Unique = isUnique;
            }
        }

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
        protected void PopulateServiceMaps(string serviceName, string childName, string nodeName)
        {
            string key = childName;
            if (!this.TypeMap.ContainsKey(key))
            {
                this.TypeMap[key] = new NodeProperties
                {
                    ServiceName = serviceName.ToLower(),
                    NodeName = nodeName.ToLower(),
                };
            }
            else
            {
                this.TypeMap[key].ServiceName = serviceName.ToLower();
                this.TypeMap[key].NodeName = nodeName.ToLower();
            }
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
            foreach (KeyValuePair<string, object> kv in root)
                merged.Add(kv);

            for (int _ = 0; _ < resultCollection.Count; ++_)
            {
                PopulationResult current = resultCollection.Dequeue();
                foreach (KeyValuePair<string, object> kv in current)
                    merged.Add(kv);
            }


            return new PopulationResult(merged);
        }

        public async Task<PopulationResult> PopulateAsync<TInput>(PopulationContext<TInput> cntx)
            where TInput : IServiceInput
        {
            if (MessageMap == null)
                MessageMap = new MessageProperties("NONE", "Id");

            PopulationResultCollection rs = await DataCollector.CollectData(this, cntx.Input);

            PopulationResult merged = MergeIntoAggregate(rs);
            return merged;
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
            AnalyticalChild<T, TProperty> child;
            child = new AnalyticalChild<T, TProperty>(directParent ?? this, name, childType, this.TypeMap);

            return child;
        }

        public void Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var expression = (MemberExpression) propertyExpression.Body;
            string name = expression.Member.Name;

            string toRemove = Name + '_' + name;
            if (typeof(TProperty).IsPrimitive || typeof(TProperty) == typeof(string))
                DeleteFromTypeMap(toRemove, true);
            else DeleteFromTypeMap(toRemove, false);
        }

        // ~AnalyticalObject(){
        //     if (FinalTypeMap.Count == 0)
        //         FinalTypeMap = TypeMap;
        // }
    }
}