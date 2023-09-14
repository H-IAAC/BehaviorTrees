using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace HIAAC.BehaviorTree
{

    /// <summary>
    /// Inspector view for the BehaviorTree editor.
    /// 
    /// Shows selected element.
    /// </summary>
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor; //Editor for showing Objects inspector

        /// <summary>
        /// Update selected element with some VisualElement.
        /// </summary>
        /// <param name="container">Element to show.</param>
        public void UpdateSelection(VisualElement container)
        {
            Clear();
            Add(container);
        }

        /// <summary>
        /// Update selected element with some object.
        /// </summary>
        /// <param name="obj">Object to create inspector.</param>
        public void UpdateSelection(UnityEngine.Object obj)
        {
            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(obj);
            IMGUIContainer container = new IMGUIContainer(OnGUIHandler);
         
            UpdateSelection(container);
        }

        
        /// <summary>
        /// Update selection with some SerializedProperty.
        /// </summary>
        /// <param name="property">Property to show in inspector.</param>
        public void UpdateSelection(SerializedProperty property)
        {
            IMGUIContainer container = new()
            {
                onGUIHandler = () =>
                {
                    if(property == null)
                    {
                        
                    }
                    else if (property.serializedObject.targetObject == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        property.serializedObject.Update();
                        EditorGUILayout.PropertyField(property, true);
                        property.serializedObject.ApplyModifiedProperties();
                    }


                }
            };

            UpdateSelection(container);
        }

        /// <summary>
        /// Handler for editor changes.
        /// </summary>
        void OnGUIHandler()
        {
            if (!editor)
            {
                return;
            }

            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        }
    }
}