using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTrees
{

    public class BlackboardView : UnityEditor.Experimental.GraphView.Blackboard
    {
        public Action<SerializedProperty> OnPropertySelect; //Action to execute on field selection

        BehaviorTree tree; //Active behavior tree

        /// <summary>
        /// View constructor.
        /// </summary>
        /// <param name="associatedGraphView">Associated BT View.</param>
        public BlackboardView(BehaviorTreeView associatedGraphView = null) : base(associatedGraphView)
        {
            SetPosition(new Rect(10, 30, 200, 300));
            scrollable = true;
            Add(new BlackboardSection { title = "Exposed Properties" });

            //Configure actions
            addItemRequested = _blackboard => AddItemRequestHandler();
            editTextRequested = (blackboard, element, newText) => EditItemRequestHandler(element, newText);
        }

        /// <summary>
        /// Draw property on the view.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        void drawProperty(BlackboardProperty property, int index)
        {
            VisualElement container = new();

            BlackboardField2 field = new()
            {
                text = property.PropertyName,
                typeText = $"{property.PropertyTypeName} property",
                OnPropertySelect = OnPropertySelect,
                tree = this.tree,
                index = index

            };

            container.Add(field);
            Add(container);

        }

        /// <summary>
        /// Create new property.
        /// </summary>
        /// <param name="type">Property type. Must inherit from BlackboardProperty.</param>
        public void CreateProperty(Type type)
        {
            //Check if have active tree
            if (tree == null)
            {
                Debug.LogError("Cannot create property without active tree asset.");
                return;
            }

            //Create property
            BlackboardProperty property = tree.CreateProperty(type);

            //Draw property on the view.
            drawProperty(property, tree.blackboard.properties.Count-1);
        }

        /// <summary>
        /// Delete an property.
        /// </summary>
        /// <param name="field">Property to delete.</param>
        public void DeleteProperty(BlackboardField field)
        {   
            //Get property index on blackboard
            string name = field.text;
            int index = tree.blackboard.properties.FindIndex(x => x.Name == name);

            //Remove property
            tree.DeleteProperty(tree.blackboard.properties[index].property);

            OnPropertySelect.Invoke(null);
        }

        /// <summary>
        /// Populate view with new behavior tree.
        /// </summary>
        /// <param name="tree">Behavior Tree to populate view.</param>
        public void PopulateView(BehaviorTree tree)
        {
            //Clear previous tree properties.
            Clear();

            this.tree = tree;
            if (tree == null)
            {
                return;
            }

            //Draw tree properties
            for(int i = 0; i<tree.blackboard.properties.Count; i++)
            {
                BlackboardOverridableProperty property = tree.blackboard.properties[i];
                drawProperty(property.property, i);
            }
        }

        // Event handlers // ----------------------------------------------------------------------------- //

        public void AddItemRequestHandler()
        {
            GenericMenu menu = new();

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(BlackboardProperty));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                menu.AddItem(new GUIContent($"{type.Name}"), false, () => CreateProperty(type));
            }

            menu.ShowAsContext();
        }

        public void EditItemRequestHandler(VisualElement element, string newText)
        {
            if (newText == "")
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            BlackboardField field = element as BlackboardField;
            string oldName = field.text;

            if (tree.blackboard.properties.Any(x => x.Name == newText))
            {
                Debug.LogError("This name is already in use.");
                return;
            }

            int index = tree.blackboard.properties.FindIndex(x => x.Name == oldName);
            tree.blackboard.properties[index].Name = newText;
            field.text = newText;
        }


    }
}