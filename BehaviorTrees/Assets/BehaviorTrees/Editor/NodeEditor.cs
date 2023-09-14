using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Custom editor for Nodes
    /// </summary>
    [CustomEditor(typeof(Node), true)]
    public class NodeEditor : Editor
    {
        /// <summary>
        /// If the properties foldout is open.
        /// </summary>
        bool showProperties = true;

        /// <summary>
        /// Properties to not draw in default draw
        /// </summary>
        static readonly string[] noDraw = new string[]{
        "useMemory",
        "useUtility", "utilitySelectionMethod", "utilityThreshould",
        "subtree"};

        /// <summary>
        /// Properties that are draw if utility composite node
        /// </summary>
        static readonly string[] utilityProperties = new string[]{
        "utilitySelectionMethod", "utilityThreshould"};

        Node node;
        SubtreeNode subtreeNode;
        RequestBehaviorNode requestBehaviorNode;

        /// <summary>
        /// Draw the node editor
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, noDraw);

            //Cast target
            node = target as Node;
            subtreeNode = node as SubtreeNode;
            requestBehaviorNode = node as RequestBehaviorNode;

            DrawMemory();

            DrawComposite();
            DrawSubtree();
           
            DrawProperties();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw memory option
        /// </summary>
        void DrawMemory()
        {
            if (node.MemoryMode == MemoryMode.Both)
            {
                node.UseMemory = EditorGUILayout.Toggle("Use memory", node.UseMemory);
            }
        }

        /// <summary>
        /// Draw composite specific properties
        /// </summary>
        void DrawComposite()
        {
            if (node is CompositeNode composite)
            {
                composite.useUtility = EditorGUILayout.Toggle("Use utility", composite.useUtility);

                if (composite.useUtility)
                {
                    foreach (string propertyName in utilityProperties)
                    {
                        if (propertyName == "utilityThreshould" &&
                            composite.utilitySelectionMethod != UtilitySelectionMethod.RANDOM_THRESHOULD)
                        {
                            continue;
                        }

                        SerializedProperty property = serializedObject.FindProperty(propertyName);
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
            }
        }

        /// <summary>
        /// Draw subtree specific properties
        /// </summary>
        void DrawSubtree()
        {
            if (subtreeNode && !requestBehaviorNode)
            {
                SerializedProperty property = serializedObject.FindProperty("subtree");
                EditorGUILayout.PropertyField(property, true);
            }
        }

        /// <summary>
        /// Draw the node properties
        /// </summary>
        void DrawProperties()
        {
            if (node.blackboard.properties.Count > 0)
            {
                showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, "Properties");
                if (showProperties)
                {
                    string[] treeBlackboardProperties = getTreeBlackboardProperties(true);

                    for (int i = 0; i < node.blackboard.properties.Count; i++)
                    {
                        BlackboardOverridableProperty property = node.blackboard.properties[i];
                        if (property.property == null)
                        {
                            Debug.LogWarning($"Property {i} of node {node.name} is null");
                            continue;
                        }

                        EditorGUILayout.LabelField(property.Name, EditorStyles.boldLabel); //Property label

                        EditorGUILayout.BeginHorizontal(); //Pass value | "Blackboard target" Map dropdown
                        {
                            //Pass value toggle if subtree and not request
                            if (subtreeNode && !requestBehaviorNode)
                            {
                                float oldWidth2 = EditorGUIUtility.labelWidth;
                                EditorGUIUtility.labelWidth = 70;
                                subtreeNode.passValue[i] = EditorGUILayout.Toggle("Pass Value", subtreeNode.passValue[i]);
                                EditorGUIUtility.labelWidth = oldWidth2;
                            }

                            //Get current map index
                            int currentIndex = 0;
                            for (int j = 0; j < treeBlackboardProperties.Length; j++)
                            {
                                string name = treeBlackboardProperties[j];
                                if (property.parentName == name)
                                {
                                    currentIndex = j;
                                    break;
                                }
                            }

                            //Create dropdown
                            float oldWidth = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 110;
                            currentIndex = EditorGUILayout.Popup("Blackboard target", currentIndex, treeBlackboardProperties);
                            EditorGUIUtility.labelWidth = oldWidth;

                            //Get value from dropdown choice
                            if (currentIndex != 0)
                            {
                                property.parentName = treeBlackboardProperties[currentIndex];
                            }
                            else
                            {
                                property.parentName = "";
                            }

                        }
                        EditorGUILayout.EndHorizontal();

                        if (property.parentName == "")
                        {
                            SerializedProperty propertyData = serializedObject.FindProperty($"blackboard.properties.Array.data[{i}].property.value");
                            Type propertyType = property.property.GetType().GetField("value", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).FieldType;

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
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                //Autoremap button if subtree and not request
                if (subtreeNode && !requestBehaviorNode)
                {
                    if (GUILayout.Button("Autoremap"))
                    {
                        subtreeNode.autoRemap();
                    }
                }
            }
        }



        /// <summary>
        /// Creates an array with all tree blackboard property names.
        /// </summary>
        /// <param name="addNone">Add a "None" name in the first position.</param>
        /// <returns>Array with blackboard property names.</returns>
        string[] getTreeBlackboardProperties(bool addNone)
        {
            List<string> blackboardPropertiesList = new();

            if(addNone)
            {
                blackboardPropertiesList.Add("None");
            }
            
            foreach (BlackboardOverridableProperty property in node.tree.blackboard.properties)
            {
                blackboardPropertiesList.Add(property.property.PropertyName);
            }
            return blackboardPropertiesList.ToArray();
        }
    }
}