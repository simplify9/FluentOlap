using SW.FluentOlap.Models;
using SW.FluentOlap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.FluentOlap.AnalyticalNode
{
    public class AnalyticalChild<TParent, T> : AnalyticalObject<T>
    {
        readonly Type childType;
        internal string parentName { get; set; }
        protected AnalyticalObject<TParent> DirectParent { get; set; }
        private InternalType sqlType;


        /// <summary>
        /// Defines an AnalyticalChild and tries to guess the type if it's a primtive type or string. It adds its parents and grandparents to keep track of them.
        /// </summary>
        /// <param name="analyticalObject"></param>
        /// <param name="childName"></param>
        /// <param name="childType"></param>
        /// <param name="typeMapsReference"></param>
        /// <param name="grandParentName"></param>
        public AnalyticalChild(AnalyticalObject<TParent> analyticalObject, string childName, Type childType,
            TypeMap typeMapsReference = null, string grandParentName = null)
            : base(analyticalObject.TypeMap, analyticalObject.minimumKeyToHierarchy)
        {
            this.DirectParent = analyticalObject;
            this.Name = childName;
            this.childType = childType;
            this.parentName = grandParentName == null
                ? analyticalObject.Name
                : grandParentName + "_" + analyticalObject.Name;
            this.TypeMap = typeMapsReference ?? base.TypeMap;
            if (TypeUtils.TryGuessInternalType(childType, out this.sqlType))
            {
                base.PopulateTypeMaps(
                    new NodeProperties() {InternalType = sqlType}, 
                    parentName, childName, true);
            }
        }


        internal new AnalyticalObject<TParent> GetDirectParent()
        {
            return DirectParent;
        }

        /// <summary>
        /// Specifies how the incoming should be cast from one value to another
        /// </summary>
        /// <param name="transformation">Transformation Value</param>
        /// <typeparam name="TCast">Inteded output type</typeparam>
        /// <returns></returns>
        public AnalyticalChild<TParent, T> HasTransformation<TCast>(Func<object, TCast> transformation)
        {
            
            PopulateTypeMaps(new NodeProperties()
            {
                Transformation = o => MasterWrappers.MasterFunctionWrapper(transformation, o),
            }, parentName, Name, true);
            
            return this;
        }

        public AnalyticalChild<TParent, T> HasTransformation(string key)
        {
            if (!FluentOlapConfiguration.TransformationsMasterList
                .TryGetValue(key, out Func<object, object> transformation))
                throw new KeyNotFoundException($"No transformation with key {key} found in TransformationMasterList.");
                    
            PopulateTypeMaps(new NodeProperties()
            {
                Transformation = transformation
            }, parentName, Name, true);
            
            return this;
        }


        /// <summary>
        /// Takes in a lambda that returns an object of the same incoming type,
        /// returning a new value after applying the transformation.
        /// If the incoming value is null, the default value will be used.
        /// </summary>
        /// <param name="transformation">Transformation Lambda</param>
        /// <param name="defaultValue">Default v</param>
        /// <returns></returns>
        public AnalyticalChild<TParent, T> HasTransformation(Func<T, T> transformation, object defaultValue)
        {
                
            PopulateTypeMaps(new NodeProperties()
            {
                Transformation = o => MasterWrappers.MasterFunctionWrapper(transformation, o, defaultValue),
            }, parentName, Name, true);
            
            return this;
        }
        /// <summary>
        /// Takes in a lambda that returns an object of the same incoming type,
        /// returning a new value after applying the transformation.
        /// If the incoming value is null, an Exception will be thrown.
        /// </summary>
        /// <param name="transformation">Transformation Lambda</param>
        /// <returns></returns>
        public AnalyticalChild<TParent, T> HasTransformation(Func<T, T> transformation)
        {
                
            PopulateTypeMaps(new NodeProperties()
            {
                Transformation = o => MasterWrappers.MasterFunctionWrapper(transformation, o),
            }, parentName, Name, true);
            
            return this;
        }

        protected string GetParentChain()
        {
            List<string> parentChainArr = new List<string>();
            var parent = DirectParent;
            parentChainArr.Insert(0, parent.Name);
            while (parent != null && parent.GetDirectParent() != null)
            {
                parentChainArr.Add(parent.Name);
                parent = parent.GetDirectParent();
            }

            return string.Join(',', parentChainArr);
        }


        /// <summary>
        /// To specify that this (primtive) property is a reference to another model found from another API or service
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public AnalyticalChild<TParent, T> GetFromService<TS>(string serviceName, AnalyticalObject<TS> node)
            where TS : class
        {
            PopulateTypeMaps(new NodeProperties()
            {
                ServiceName = serviceName,
                NodeName = node.Name,
            }, parentName, Name, true);

            foreach (var entry in node.TypeMap)
            {
                string key = Namer.EnsureMinimumUniqueKey(Name.ToLower() + "_" + entry.Key, TypeMap, true);
                TypeMap[key] = entry.Value;
            }

            return this;
        }

        public void DeleteFromTypemaps(string name, bool isPrimitive)
        {
            DeleteFromTypeMap(parentName + '_' + name, isPrimitive);
        }

        /// <summary>
        /// Override default behavior to modify parent passing
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="directParent"></param>
        /// <returns></returns>
        public new AnalyticalChild<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            AnalyticalObject<T> directParent = null)
        {
            var expression = (MemberExpression) propertyExpression.Body;
            string name = expression.Member.Name;
            Type childType = propertyExpression.ReturnType;
            AnalyticalChild<T, TProperty> child;
            // Passing the type maps down to the child, so that all children/grandchildren share the same type dictionary
            child = new AnalyticalChild<T, TProperty>(this, name, childType, this.TypeMap, this.parentName);
            return child;
        }
    }
}