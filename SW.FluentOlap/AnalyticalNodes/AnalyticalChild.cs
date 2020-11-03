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
        public string parentName { get; set; }
        protected AnalyticalObject<TParent> DirectParent { get; set; }
        private bool isUnique;
        private InternalType sqlType;


        /// <summary>
        /// Defines an AnalyticalChild and tries to guess the type if it's a primtive type or string. It adds its parents and grandparents to keep track of them.
        /// </summary>
        /// <param name="analyticalObject"></param>
        /// <param name="childName"></param>
        /// <param name="childType"></param>
        /// <param name="typeMapsReference"></param>
        /// <param name="grandParentName"></param>
        public AnalyticalChild(AnalyticalObject<TParent> analyticalObject, string childName, Type childType, TypeMap typeMapsReference = null, string grandParentName = null) : base(analyticalObject.TypeMap)
        {
            this.DirectParent = analyticalObject;
            this.Name = childName;
            this.childType = childType;
            this.parentName = grandParentName == null? analyticalObject.Name : grandParentName + "_" + analyticalObject.Name;
            this.TypeMap = typeMapsReference ?? base.TypeMap;
            if(TypeUtils.TryGuessInternalType(childType, out this.sqlType))
            {
                PopulateTypeMaps(sqlType, childName);
            }
        }


        public new AnalyticalObject<TParent> GetDirectParent()
        {
            return DirectParent;
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
        public AnalyticalChild<TParent, T> GetFromService<TS>(string serviceName, AnalyticalObject<TS> node) where TS : class
        {
            PopulateServiceMaps(serviceName, parentName + "_" + Name, node.Name);
            foreach(var entry in node.TypeMap)
            {
                this.TypeMap.Add(entry);
            }
            return this;
        }

        /// <summary>
        /// Override the default population to mention parent
        /// </summary>
        /// <param name="type"></param>
        /// <param name="childName"></param>
        public new void PopulateTypeMaps(InternalType type, string childName)
        {
            base.PopulateTypeMaps(type, parentName + "_" + childName);
        }

        public void DeleteFromTypemaps(string name, bool isPrimitive)
        {
            DeleteFromTypeMap(parentName + '_' + name, isPrimitive);
        }
        
        public new void Ignore<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var parentChain = GetParentChain() + '_' + Name;
            var expression = (MemberExpression)propertyExpression.Body;
            string name = parentChain + '_' + expression.Member.Name;
            
            if (typeof(TProperty).IsPrimitive || typeof(TProperty) == typeof(string))
                DeleteFromTypeMap(name, true);
            else DeleteFromTypeMap(name, false);

        }

        /// <summary>
        /// Override default behavior to modify parent passing
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="directParent"></param>
        /// <returns></returns>
        public new AnalyticalChild<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, AnalyticalObject<T> directParent = null)
        {

            var expression = (MemberExpression)propertyExpression.Body;
            string name = expression.Member.Name;
            Type childType = propertyExpression.ReturnType;
            AnalyticalChild<T, TProperty> child;
            // Passing the type maps down to the child, so that all children/grandchildren share the same type dictionary
            child = new AnalyticalChild<T, TProperty>(this, name, childType, this.TypeMap, this.parentName);
            return child;
        }

        /// <summary>
        /// Define SQL type for this property
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public AnalyticalChild<TParent, T> HasSqlType(InternalType sqlType)
        {
            PopulateTypeMaps(sqlType, Name);
            return this;
        }

        /// <summary>
        /// Specify that this property is unique
        /// </summary>
        /// <returns></returns>
        [Obsolete("Function no longer usable", true)]
        public AnalyticalChild<TParent, T> IsUnique()
        {
            isUnique = true;
            base.IsUnique(isUnique, parentName + "_" + Name);
            return this;
        }

    }
}
