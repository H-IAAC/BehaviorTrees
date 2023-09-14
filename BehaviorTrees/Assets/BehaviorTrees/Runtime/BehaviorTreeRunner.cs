using System;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Runs the tree associating it to the object
    /// </summary>
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [Tooltip("Tree to execute.")][SerializeField] public BehaviorTree tree;

        /// <summary>
        /// Initializes the tree
        /// </summary>
        void Start()
        {
            tree = tree.Clone();
            tree.Bind(gameObject);
        }

        /// <summary>
        /// Run the tree
        /// </summary>
        void Update()
        {
            tree.Update();
        }

        /// <summary>
        /// Set some tree property value.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value to set.</param>
        /// <exception cref="ArgumentException">If property does not exist.</exception>
        public void SetBlackboardProperty(string name, object value)
        {
            tree.SetPropertyValue(name, value);
        }

        /// <summary>
        /// Get some tree property value.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="ArgumentException">If property does not exist.</exception>
        public T GetBlackboardProperty<T>(string name)
        {
            return tree.blackboard.GetPropertyValue<T>(name);
        }

        /// <summary>
        /// Get some tree property value.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Property value.</returns>
        /// <exception cref="ArgumentException">If property does not exist.</exception>
        /*public object GetBlackboardProperty(string name)
        {
            int index = tree.blackboard.FindIndex(x => x.PropertyName == name);
            if (index < 0)
            {
                throw new ArgumentException("Property does not exist in tree.");
            }

            return tree.blackboard[index].Value;
        }*/
    }
}