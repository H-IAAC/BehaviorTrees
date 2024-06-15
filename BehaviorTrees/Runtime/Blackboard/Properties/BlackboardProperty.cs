using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Main class for creating blackboard properties.
    /// </summary>
    /// <remarks>
    /// Important: all subclasses must store the property value on a variable name "value" to show in the editor.
    /// </remarks>
    [Serializable]
    public abstract class BlackboardProperty
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string PropertyName;

        /// <summary>
        /// Name of the type of the property.
        /// </summary>
        public abstract string PropertyTypeName { get; }
        
        /// <summary>
        /// Value of the property.
        /// </summary>
        public abstract object Value { get; set; }

        /// <summary>
        /// Create a clone of the property, with same name and value.
        /// </summary>
        /// <returns>Copy of the property.</returns>
        public virtual BlackboardProperty Clone()
        {
            BlackboardProperty clone = CreateInstance(this.GetType());
            clone.PropertyName = PropertyName;
            clone.Value = Value;

            return clone;
        }
        

        /// <summary>
        /// Create a instance of some property by the type.
        /// </summary>
        /// <typeparam name="T">Type of property to create.</typeparam>
        /// <returns>Created property.</returns>
        public static BlackboardProperty CreateInstance<T>()
        {
            return Activator.CreateInstance(typeof(T)) as BlackboardProperty;
        }

        /// <summary>
        /// Create a instance of some property by the type.
        /// </summary>
        /// <param name="type">Type of property to create.</param>
        /// <returns>Created property.</returns>
        public static BlackboardProperty CreateInstance(Type type)
        {
            return Activator.CreateInstance(type) as BlackboardProperty;
        }

        /// <summary>
        /// Can be used for validating the property value after changing on the inspector.
        /// </summary>
        public virtual void Validate()
        {

        }

    }

    /// <summary>
    /// Generic BlackboardProperty for creating some typed property.
    /// </summary>
    /// <typeparam name="T">Type of the property to create.</typeparam>
    [Serializable]
    public abstract class BlackboardProperty<T> : BlackboardProperty
    {
        [SerializeField][Tooltip("Value of the property.")]
        T value = default;

        /// <summary>
        /// Value of the property.
        /// </summary>
        public override object Value
        {
            get { return value; }
            set { this.value = (T)value; }
        }

        /// <summary>
        /// Name of the type of the property.
        /// </summary>
        public override string PropertyTypeName
        {
            get
            {
                //Get name of the type
                string name = typeof(T).Name;

                //Remove "Property" sufix if any.
                string sufix = "Property";
                if (name.EndsWith(sufix))
                {
                    name = name.Substring(0, name.Length - sufix.Length);
                }

                return name;
            }
        }

    }

    [Serializable]
    public abstract class ObjectBlackboardProperty<T> : BlackboardProperty where T : UnityEngine.Object 
    {
        [SerializeField]
        UnityEngine.Object value;

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is T)
                {
                    this.value = value as UnityEngine.Object;
                }
                else
                {
                    this.value = null;
                }
            }
        }

        public T ObjectValue
        {
            get
            {
                return value as T;
            }
        }

        /// <summary>
        /// Name of the type of the property.
        /// </summary>
        public override string PropertyTypeName
        {
            get
            {
                //Get name of the type
                string name = typeof(T).Name;

                //Remove "Property" sufix if any.
                string sufix = "Property";
                if (name.EndsWith(sufix))
                {
                    name = name.Substring(0, name.Length - sufix.Length);
                }

                return name;
            }
        }

        public override void Validate()
        {
            if (value is not T)
            {
                value = null;
            }
        }
    }
}
