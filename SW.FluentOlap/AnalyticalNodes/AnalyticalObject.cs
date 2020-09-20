using SW.FluentOlap.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using SW.FluentOlap.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SW.FluentOlap.AnalyticalNode
{
    /// <summary>
    /// Object that keeps track of mappings for this object's children properties (and their children)
    /// </summary>
    /// <typeparam name="T">Type of object ot be analyzed</typeparam>
    public class AnalyticalObject<T> : IAnalyticalNode
    {
        private static TypeMap FinalTypeMap { get; set; } = new TypeMap();
        public TypeMap TypeMap { get; protected set; } = new TypeMap();
        public string ServiceName { get; set; }
        public MessageProperties MessageMap { get; set; }
        public string Name { get; set; }
        public Type AnalyzedType { get; set; }
        public AnalyticalObject()
        {
            this.AnalyzedType = typeof(T);
            this.Name = AnalyzedType.Name;
            AnalyzedType = typeof(T);
            InitTypeMap();
        }

        private void InitTypeMap(Type typeToInit = null, string prefix = null, string preferredName = null, string directParentName = null)
        {
            if (typeToInit == null) typeToInit = this.AnalyzedType;
            if (prefix == null) prefix = this.AnalyzedType.Name;
            if (preferredName == null) preferredName = typeToInit.Name;
            if (directParentName == null) directParentName = typeToInit.Name;

            if(typeToInit.IsPrimitive || typeToInit == typeof(string))
                PopulateTypeMaps(TypeGuesser.GuessType(typeToInit), $"{prefix}_{preferredName}");
            else 
            {
                    if (prefix != directParentName) prefix = $"{prefix}_{directParentName}";
                    foreach(var prop in typeToInit.GetProperties())
                        InitTypeMap(prop.PropertyType, $"{prefix}", prop.Name);
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
                    SqlType = type
                };
            }
            else
            {
                this.TypeMap[key].SqlType = type;
            }
        }

        public AnalyticalObject<T> GetDirectParent()
        {
            return null;
        }

        /// <summary>
        /// Add a new map  by setting it to unique
        /// Or update the UNIQUE property if it exists
        /// </summary>
        /// <param name="isUnique"></param>
        /// <param name="childName"></param>
        public void IsUnique(bool isUnique, string childName)
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
                foreach(var entry in this.TypeMap)
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
            var expression = (MemberExpression)propertyExpression.Body;
            string keyPath = expression.Member.Name;
            return this.Handles(messageName, keyPath);
        }

        public AnalyticalObject<T> GetFromService(string serviceName)
        {
            this.ServiceName = serviceName;
            return this;
        }

        public async Task<PopulationResult> PopulateAsync<TM>(PopulationContext<TM> cntx)
        {
            string val = cntx.Message.ToString();
            if (typeof(TM).Name == this.MessageMap.MessageName)
            {
                JToken tok = JToken.FromObject(cntx.Message);
                val = tok[this.MessageMap.KeyPath].ToString();
            }
            var collector = new DataCollector();
            PopulationResult rs = await collector.GetDataFromEndpoints(
                val,
                this.ServiceName,
                FluentOlapConfiguration.ServiceDefinitions,
                AnalyticalObject<T>.FinalTypeMap.Count == 0 ? this.TypeMap : AnalyticalObject<T>.FinalTypeMap,
                null,
                cntx.HttpClientFactory
            );
            rs.TargetTable = this.GetType().Name;
            return rs;
        }


        /// <summary>
        /// Pass in a property by LINQ expression so that it can be defined by modifying the resulting AnalyticalChild
        /// The AnalyticalChild constructor will try to guess the type.
        /// </summary>
        /// <typeparam name="TProperty">Type of property to be defined</typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="directParent">Direct parent, this should not be set by the user</param>
        /// <returns></returns>
        public AnalyticalChild<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, AnalyticalObject<T> directParent = null)
        {
            var expression = (MemberExpression)propertyExpression.Body;
            string name = expression.Member.Name;
            Type childType = propertyExpression.ReturnType;
            AnalyticalChild<T, TProperty> child;
            child = new AnalyticalChild<T, TProperty>(directParent ?? this, name, childType, this.TypeMap);

            return child;
        }

        public void Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var expression = (MemberExpression)propertyExpression.Body;
            string name = expression.Member.Name;

            string toRemove = Name + '_' + name;
            if (typeof(TProperty).IsPrimitive || typeof(TProperty) == typeof(string))
                DeleteFromTypeMap(toRemove, true);
            else DeleteFromTypeMap(toRemove, false);

        }

        ~AnalyticalObject(){
            lock (AnalyticalObject<T>.FinalTypeMap)
            {
                if(AnalyticalObject<T>.FinalTypeMap.Count == 0)
                {
                    AnalyticalObject<T>.FinalTypeMap = this.TypeMap;
                }
            }
        }
    }
}
