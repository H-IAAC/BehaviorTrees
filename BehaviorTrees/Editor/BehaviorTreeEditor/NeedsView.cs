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
                width = 120
            };

            Column c1 = new()
            {
                name = "value",
                title = "Value",
                width = 160
            };

            Columns columns = new();
            columns.Add(c0);
            columns.Add(c1);

            listView = new(columns);
            listView.showAddRemoveFooter = false;
            listView.itemsSource = new List<NeedValue>();

            listView.columns["need"].makeCell = () => new IMGUIContainer();
            listView.columns["value"].makeCell = () => new Slider() { highValue = 1f };

            listView.columns["need"].bindCell = (VisualElement element, int index) => {
                IMGUIContainer container = element as IMGUIContainer;
                container.Clear();

                if(tree == null)
                {
                    return;
                }

                container.onGUIHandler = () =>
                {
                    SerializedProperty property = serializedTree.FindProperty($"needsContainer.needs.Array.data[{index}].need");

                    if (property == null)
                    {

                    }
                    else if (property.serializedObject.targetObject == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        property.serializedObject.Update();
                        EditorGUILayout.PropertyField(property, GUIContent.none);
                        property.serializedObject.ApplyModifiedProperties();
                    }


                };

            };

            listView.columns["value"].bindCell = (VisualElement element, int index) => {
                Slider slider = element as Slider;
                slider.value = tree.needsContainer.needs[index].value;
                slider.showInputField = true;

                slider.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    tree.needsContainer.needs[index].value = evt.newValue;
                });
            };

            this.Add(listView);

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


    }
}