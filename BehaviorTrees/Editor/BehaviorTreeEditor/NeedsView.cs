using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTrees.Needs
{
    public class NeedsView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NeedsView, VisualElement.UxmlTraits> { }

        MultiColumnListView listView;

        List<NeedValue> needs;
        BehaviorTree tree;
        SerializedObject serializedTree;

        public NeedsView()
        {

            Column c0 = new()
            {
                name = "need",
                title = "Need",
                width = 100
            };

            Column c1 = new()
            {
                name = "value",
                title = "Value",
                width = 130
            };

            Column c2 = new()
            {
                name = "weight",
                title = "Weight",
                width = 55
            };

            Columns columns = new()
            {
                c0,
                c1,
                c2
            };

            listView = new(columns)
            {
                showAddRemoveFooter = false,
                itemsSource = new List<NeedValue>()
            };


            listView.columns["need"].makeCell = () => new IMGUIContainer();
            listView.columns["value"].makeCell = () => new Slider() { highValue = 1f };
            listView.columns["weight"].makeCell = () => new IMGUIContainer();

            listView.columns["need"].bindCell = (VisualElement element, int index) => BindIMGUI(element, index, $"needsContainer.needs.Array.data[{index}].need");
            
            listView.columns["value"].bindCell = (VisualElement element, int index) => {
                Slider slider = element as Slider;
                
                if(tree.needsContainer.needs[index] == null)
                {
                    Debug.Log("Null need");
                    return;
                }

                slider.value = tree.needsContainer.needs[index].value;
                slider.showInputField = true;

                slider.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    tree.needsContainer.needs[index].value = evt.newValue;
                });
            };

            listView.columns["weight"].bindCell = (VisualElement element, int index) => BindIMGUI(element, index, $"needsContainer.needs.Array.data[{index}].weight");

            listView.itemsAdded += (IEnumerable<int> indexes) => {
                foreach(int index in indexes)
                {
                    schedule.Execute(() => Refresh(index)).Every(100).ForDuration(101);
                }
            };

            Add(listView);

        }

        public void Refresh(int index)
        {
            serializedTree.Update();

            listView.RefreshItem(index);
        }

        public void PopulateView(BehaviorTree tree)
        {
            if (tree == null)
                return;

            this.tree = tree;
            serializedTree = new(tree);
            needs = tree.needsContainer.needs;

            listView.itemsSource = needs;
            listView.Rebuild();            
            listView.showAddRemoveFooter = true;
        }

        public void BindIMGUI(VisualElement element, int index, string propertyName)
        {
            IMGUIContainer container = element as IMGUIContainer;
            container.Clear();

            if (tree == null)
            {
                return;
            }

            container.onGUIHandler = () =>
            {
                SerializedProperty property = serializedTree.FindProperty(propertyName);

                if (property == null)
                {
                    Debug.Log($"Null property {propertyName}");
                }
                else if (property.serializedObject.targetObject == null)
                {
                    container.Clear();
                }
                else
                {
                    property.serializedObject.Update();
                    EditorGUILayout.PropertyField(property, GUIContent.none);
                    property.serializedObject.ApplyModifiedProperties();
                }

            };
        }
    }
}