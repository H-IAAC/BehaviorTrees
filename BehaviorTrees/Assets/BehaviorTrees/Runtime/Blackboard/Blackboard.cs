using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Blackboard for store tree of node properties.
    /// </summary>
    [Serializable]
    public class Blackboard
    {
        [SerializeField][Tooltip("Stored properties.")] public List<BlackboardOverridableProperty> properties;

        /// <summary>
        /// Parent blackboard, used when mapping properties.
        /// </summary>
        public Blackboard parent = null;

        /// <summary>
        /// Binded object (tree or node).
        /// </summary>
        UnityEngine.Object baseObject;

        /// <summary>
        /// Blackboard constructor.
        /// </summary>
        /// <param name="baseObject">Object the blackboard belongs</param>
        public Blackboard(UnityEngine.Object baseObject)
        {
            properties = new();
            this.baseObject = baseObject;
        }

        /// <summary>
        /// Create a new property on the blackboard.
        /// </summary>
        /// <param name="type">Type of the property.</param>
        /// <param name="name">Name of the property. Can be changed internally for avoiding duplicates.</param>
        /// <returns>Created property.</returns>
        public BlackboardProperty CreateProperty(Type type, string name)
        {
            //Avoid duplicated name
            while (properties.Any(x => x.Name == name))
            {
                name += "(1)";
            }

            //Create property
            BlackboardProperty property = BlackboardProperty.CreateInstance(type);
            property.PropertyName = name;

            //Create associated overridable property
            BlackboardOverridableProperty op = new()
            {
                parentName = "",
                property = property
            };
            properties.Add(op);

            return property;
        }

        /// <summary>
        /// Create a new property on the blackboard.
        /// 
        /// The name is defined as the PropertyTypeName.
        /// </summary>
        /// <param name="type">Type of the property.</param>
        /// <returns>Created property.</returns>
        public BlackboardProperty CreateProperty(Type type)
        {
            BlackboardProperty property = BlackboardProperty.CreateInstance(type);
            string name = property.PropertyTypeName;

            return CreateProperty(type, name);
        }

        /// <summary>
        /// Retrieves a property in the blackboard
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="forceNodeProperty">If true, don't retrieve the parent blackboard if mapped.</param>
        /// <returns>Property with given name.</returns>
        /// <exception cref="ArgumentException">If property with name does not exist.</exception>
        public BlackboardProperty GetProperty(string name, bool forceNodeProperty = false)
        {
            int index = properties.FindIndex(x => x.Name == name);

            if (index < 0)
            {
                throw new ArgumentException("Property does not exist in node.");
            }

            string parentName = properties[index].parentName;
            if (parentName == "" || forceNodeProperty)
            {
                return properties[index].property;
            }
            else
            {
                return parent.GetProperty(parentName);
            }
        }

        /// <summary>
        /// Get the current value of a property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent blackboard.</param>
        /// <returns>Value of the property.</returns>
        public object GetPropertyValue(string name, bool forceNodeProperty = false)
        {
            return GetProperty(name, forceNodeProperty).Value;
        }

        /// <summary>
        /// Get the current value of a property.
        /// </summary>
        /// <typeparam name="T">Type of the property to cast when returning.</typeparam>
        /// <param name="name">Name of the property</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent blackboard if mapped.</param>
        /// <returns>Value of the property.</returns>
        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {
            return (T)GetProperty(name, forceNodeProperty).Value;
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">New value.</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent blackboard if mapped.</param>
        public void SetPropertyValue<T>(string name, T value, bool forceNodeProperty = false)
        {
            GetProperty(name, forceNodeProperty).Value = value;
        }

        /// <summary>
        /// Deletes all properties on the blackboard.
        /// </summary>
        /// <param name="dontDelete">List of property's names to not delete.</param>
        public void ClearPropertyDefinitions(List<string> dontDelete = null)
        {

            if (dontDelete != null)
            {
                properties.RemoveAll(x => !dontDelete.Contains(x.Name));
            }
            else
            {
                properties.Clear();
            }

        }

        /// <summary>
        /// Checks if blackboard have property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if blackboard has property with given name.</returns>
        public bool HasProperty(string name)
        {
            return properties.Any(x => x.Name == name);
        }

        /// <summary>
        /// Remove a property from the blackboard.
        /// </summary>
        /// <param name="property">Property to remove.</param>
        public void DeleteProperty(BlackboardProperty property)
        {
#if UNITY_EDITOR
            Undo.RecordObject(baseObject, "Behavior Tree (DeleteTreeProperty)");
#endif

            int index = properties.FindIndex(x => x.property == property);
            properties.RemoveAt(index);

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif

        }

    }
}