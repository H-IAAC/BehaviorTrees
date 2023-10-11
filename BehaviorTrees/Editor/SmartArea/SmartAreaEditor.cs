using UnityEditor;
using HIAAC.BehaviorTrees;
using UnityEngine;
using UnityEditor.Rendering;
using System.IO;
using UnityEngine.SceneManagement;

namespace HIAAC.BehaviorTrees.SmartAreas
{

    [CustomEditor(typeof(SmartArea), true)]
    public class SmartAreaEditor : Editor
    {
        static class Styles
        {
            public static readonly GUIContent container = EditorGUIUtility.TrTextContent("Container", "BTagContainer.");

            public static readonly GUIContent newLabel = EditorGUIUtility.TrTextContent("New", "Create a new profile.");
            public static readonly GUIContent cloneLabel = EditorGUIUtility.TrTextContent("Clone", "Create a new profile and copy the content of the currently assigned profile.");
        }

        static readonly string[] noDraw = new string[]{"tagContainer"};

        bool showContainer;

        public override void OnInspectorGUI()
        {
            SmartArea area = target as SmartArea;
            SerializedProperty tagContainerProperty = serializedObject.FindProperty("tagContainer");


            DrawPropertiesExcluding(serializedObject, noDraw);

            //VolumeEditor e;

            bool showClone = false;

            if(area.TagContainer != null)
            {
                showClone = true;
            }

            
            int buttonWidth = showClone ? 45 : 60;
            float indentOffset = EditorGUI.indentLevel * 15f;
            Rect lineRect = EditorGUILayout.GetControlRect();
            Rect labelRect = new(lineRect.x, lineRect.y, EditorGUIUtility.labelWidth - indentOffset - 3, lineRect.height);
            Rect fieldRect = new(labelRect.xMax + 5, lineRect.y, lineRect.width - labelRect.width - buttonWidth * (showClone ? 2 : 1) - 5, lineRect.height);
            Rect buttonNewRect = new(fieldRect.xMax, lineRect.y, buttonWidth, lineRect.height);
            Rect buttonCopyRect = new(buttonNewRect.xMax, lineRect.y, buttonWidth, lineRect.height);


            EditorGUI.ObjectField(fieldRect, tagContainerProperty, Styles.container);
            EditorGUI.PrefixLabel(labelRect, Styles.container);
            area.TagContainer = (BTagContainer)EditorGUI.ObjectField(fieldRect, area.TagContainer, typeof(BTagContainer), false);

            string targetName = area.name + " TagContainer";
            Scene scene = area.gameObject.scene;

            if (GUI.Button(buttonNewRect, Styles.newLabel, showClone ? EditorStyles.miniButtonLeft : EditorStyles.miniButton))
            {
                

                BTagContainer asset = CreateAssetAt<BTagContainer>(scene, targetName);
                area.TagContainer = asset;
            }
            if (showClone && GUI.Button(buttonCopyRect, Styles.cloneLabel, EditorStyles.miniButtonRight))
            {
                BTagContainer newContainer = CreateAssetAt<BTagContainer>(scene, targetName, area.TagContainer);
                area.TagContainer = newContainer;


            }

            if(area.TagContainer != null)
            {
                showContainer = EditorGUILayout.BeginFoldoutHeaderGroup(showContainer, "Container properties");
                EditorGUILayout.EndFoldoutHeaderGroup();

                if(showContainer)
                {
                    EditorGUI.indentLevel++;

                    Editor containerWindows = Editor.CreateEditor(area.TagContainer);
                    containerWindows.OnInspectorGUI();

                    EditorGUI.indentLevel--;
                }

                
                
            }
            

            serializedObject.ApplyModifiedProperties();
        }

        static T CreateAssetAt<T>(Scene scene, string targetName, T toCopy =null) where T : ScriptableObject
        {
            string text;
            if (string.IsNullOrEmpty(scene.path))
            {
                text = "Assets/";
            }
            else
            {
                string text2 = string.Concat(Path.GetDirectoryName(scene.path), str2: scene.name, str1: Path.DirectorySeparatorChar.ToString());
                if (!AssetDatabase.IsValidFolder(text2))
                {
                    string[] array = text2.Split(Path.DirectorySeparatorChar);
                    string text3 = "";
                    string[] array2 = array;
                    foreach (string text4 in array2)
                    {
                        string text5 = text3 + text4;
                        if (!AssetDatabase.IsValidFolder(text5))
                        {
                            AssetDatabase.CreateFolder(text3.TrimEnd(Path.DirectorySeparatorChar), text4);
                        }

                        text3 = text5 + Path.DirectorySeparatorChar;
                    }
                }

                text = text2 + Path.DirectorySeparatorChar;
            }

            T val;

            if(toCopy != null)
            {
                val = ScriptableObject.Instantiate(toCopy);
                Debug.Log($"{val.name}|{toCopy.name}");
            }
            else
            {
                val = ScriptableObject.CreateInstance<T>();
                val.name = targetName;
            }

            text = text + val.name.ReplaceInvalidFileNameCharacters() + ".asset";
            text = AssetDatabase.GenerateUniqueAssetPath(text);

            

            AssetDatabase.CreateAsset(val, text);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return val;
        }
    }
}