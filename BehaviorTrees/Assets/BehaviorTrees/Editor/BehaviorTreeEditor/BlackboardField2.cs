using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// BlackboardField with actions on select and delete.
    /// </summary>
    public class BlackboardField2 : BlackboardField
    {
        public Action<SerializedProperty> OnPropertySelect; //Action to execute on field selected
        
        /// <summary>
        /// Call on select action
        /// </summary>
        public override void OnSelected()
        {
            OnPropertySelect(serializedProperty);
        }

        public BehaviorTree tree; //Tree the property belongs
        public int index; //Index of property in the tree

        /// <summary>
        /// Get the serialized property for the value of the property
        /// </summary>
        public SerializedProperty serializedProperty
        {
            get
            {
                SerializedObject serializedObject = new(tree);
                return serializedObject.FindProperty($"blackboard.properties.Array.data[{index}].property.value");
            }
        }


    }
}