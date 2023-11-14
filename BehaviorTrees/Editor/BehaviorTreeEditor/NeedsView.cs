using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTrees.Needs
{
    public class NeedsView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NeedsView, VisualElement.UxmlTraits> { }


        List<string> items;

        MultiColumnListView listView;

        public NeedsView()
        {
            items = new();

            Column c0 = new();
            Column c1 = new();

            c0.title = "Need";
            c0.name = "need";

            c1.title = "Value";
            c1.name = "value";

            Columns columns = new();
            columns.Add(c0);
            columns.Add(c1);

            listView = new(columns);
            listView.showAddRemoveFooter = true;
            listView.itemsSource = items;

            listView.columns["need"].makeCell = () => new IMGUIContainer();
            listView.columns["value"].makeCell = () => new ProgressBar();

            listView.columns["need"].bindCell = (VisualElement element, int index) => {
                
            };

            listView.columns["value"].bindCell = (VisualElement element, int index) => {
                (element as ProgressBar).value = 50;
            };

            this.Add(listView);

        }

        public void OnInspectorUpdate()
        {
            //Debug.Log(items.Count);
        }


    }
}