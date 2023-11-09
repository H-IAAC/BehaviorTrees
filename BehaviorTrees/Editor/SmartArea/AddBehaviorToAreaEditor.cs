using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

namespace HIAAC.BehaviorTrees.SmartAreas
{

    [CustomEditor(typeof(AddBehaviorToArea), true)]
    public class AddBehaviorToAreaEditor : Editor
    {
        static readonly string[] noDraw = new string[]{"blackboard"};

        bool showProperties = false;

        AddBehaviorToArea realTarget;
        
        public override void OnInspectorGUI()
        {
            realTarget = target as AddBehaviorToArea;
            realTarget.OnValidate();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, noDraw);

            DrawBlackboard();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawBlackboard()
        {
            if (realTarget.blackboard.properties.Count > 0)
            {
                showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, "Properties");
                EditorGUILayout.EndFoldoutHeaderGroup();
                if (showProperties)
                {
                    EditorGUI.indentLevel++;

                    //string[] treeBlackboardProperties = getTreeBlackboardProperties(true);

                    for (int i = 0; i < realTarget.blackboard.properties.Count; i++)
                    {
                        EditorGUI.indentLevel++;

                        BlackboardOverridableProperty property = realTarget.blackboard.properties[i];
                        if (property.property == null)
                        {
                            Debug.LogWarning($"Property {i} of node {realTarget.name} is null");
                            continue;
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(property.Name, EditorStyles.boldLabel); //Property label
                            
                            float oldWidth2 = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 100;
                            realTarget.passValue[i] = EditorGUILayout.Toggle("Pass Value", realTarget.passValue[i]);
                            EditorGUIUtility.labelWidth = oldWidth2;
                        }
                        EditorGUILayout.EndHorizontal();


                        if (property.parentName == "")
                        {                      
                            SerializedProperty propertyData = serializedObject.FindProperty($"blackboard.properties.Array.data[{i}].property.value");

                            FieldInfo valueField = property.property.GetType().GetField("value", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            if(valueField == null)
                            {
                                valueField = typeof(BlackboardProperty<>).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
                            }
                            Type propertyType = valueField.FieldType;

                            if(propertyType.IsSubclassOf(typeof(UnityEngine.Object)) || propertyType == typeof(UnityEngine.Object))
                            {
                                EditorGUILayout.ObjectField(propertyData);
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propertyData, true);
                            }

                            property.property.Validate();
                        }

                        EditorGUILayout.Space();
                        
                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}