using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Double click manipulator for the BehaviorTree view.
    /// </summary>
    public class BTViewDoubleClick : MouseManipulator
    {
        double time; //Time since last mouse down.
        double doubleClickDuration = 0.3; //Max duration to detect double click
        BehaviorTreeView view; //Associated BT View

        /// <summary>
        /// Manipulator constructor.
        /// </summary>
        /// <param name="view">Associated BT View of the manipulator</param>
        public BTViewDoubleClick(BehaviorTreeView view) : base()
        {
            this.view = view;
        }

        /// <summary>
        /// Register callback for MouseDownEvent
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        /// <summary>
        /// Unregister callback for MouseDownEvent
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        //Handle mouse down event.
        void OnMouseDown(MouseDownEvent evt)
        {
            //Is not clicking on BT view
            if (target is not BehaviorTreeView)
            {
                return;
            }

            //Check if double click
            double duration = EditorApplication.timeSinceStartup - time;
            if (duration < doubleClickDuration)
            {
                //Get clicked NodeView if any 
                NodeView clickedElement = evt.target as NodeView;
                if (clickedElement == null)
                {
                    VisualElement ve = evt.target as VisualElement;
                    clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                    if (clickedElement == null)
                    {
                        return;
                    }

                }

                //Show subtree if clicked element is view of SubtreeNode 
                if (clickedElement.node is SubtreeNode subtreeNode)
                {
                    view.ToggleSubtreeView(subtreeNode);
                }

            }

            //Update last click time
            time = EditorApplication.timeSinceStartup;

        }
    }
}