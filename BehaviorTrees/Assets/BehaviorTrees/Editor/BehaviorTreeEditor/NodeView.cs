using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace HIAAC.BehaviorTree
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node; //Associated node

        public Port input; //Input port
        public Port output; //Output port

        Vector2 positionOffset = Vector2.zero; //Position offset
        Vector2 positionBase = Vector2.zero; //Base position (same as node)

        bool runtime; //If view is runtime (update state)
        bool ghost; //If view is ghost (disable selection)

        public Action<NodeView> OnNodeSelected; //Action for when view is selected

    
        static string UIFilePath = getUIFilePath(); //Path of UI file

        /// <summary>
        /// Get path of UI file.
        /// </summary>
        /// <returns>UI file path.</returns>
        static string getUIFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset NodeView");
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }


        /// <summary>
        /// If is ghost view (selecting disabled and ghost style).
        /// </summary>
        public bool Ghost
        {
            get { return ghost; }
        }
        
        /// <summary>
        /// If can select view.
        /// </summary>
        public bool Selectable
        {
            set
            {
                if (value)
                {
                    capabilities |= Capabilities.Selectable;
                }
                else
                {
                    capabilities &= ~Capabilities.Selectable;
                }
            }
            get
            {
                return IsSelectable();
            }
        }

        /// <summary>
        /// View position offset. 
        /// </summary>
        public Vector2 PositionOffset
        {
            get
            {
                return positionOffset;
            }

            set
            {
                positionOffset = value;

                UpdateViewPosition();
            }
        }

        /// <summary>
        /// NodeView constructor. Configures the view.
        /// </summary>
        /// <param name="node">Associated node for the view.</param>
        /// <param name="runtime">If is runtime (update node state). </param>
        /// <param name="ghost">If is ghost (disable selecting and ghost style).</param>
        public NodeView(Node node, bool runtime, bool ghost = false) : base(UIFilePath)
        {
            this.node = node;

            this.runtime = runtime;
            this.ghost = ghost;
            viewDataKey = node.guid;
            
            //Disable selecting if ghost
            if (ghost)
            {
                Selectable = false;
            }

            //Set view title (without "Node" sufix if any)
            title = node.name;
            if (title.EndsWith("Node"))
            {
                title = title.Substring(0, title.Length - 4);
            }

            //Configure position
            positionBase = new Vector2(node.position.x, node.position.y);
            UpdateViewPosition();

            //Configure other view properties
            ConfigureClasses();
            CreateInputPorts();
            CreateOutputPorts();
            ConfigurePortPicking();
            ConfigureLabel();
        }


        
        /// <summary>
        /// On Select handler.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();

            OnNodeSelected?.Invoke(this);
        }

        /// <summary>
        /// Update the view state
        /// </summary>
        public void UpdateState()
        {
            //NodeState styles
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");
            if (Application.isPlaying && runtime)
            {
                switch (node.state)
                {
                    case NodeState.Runnning:
                        if (node.started)
                        {
                            AddToClassList("running");
                        }
                        break;
                    case NodeState.Failure:
                        AddToClassList("failure");
                        break;
                    case NodeState.Success:
                        AddToClassList("success");
                        break;
                }

                Label utilityValue = (Label)(VisualElement)this.Query("utility-value");
                utilityValue.text = node.GetUtility().ToString("#0.00");
            }

            //Memory styles
            if (node.UseMemory)
            {
                AddToClassList("memoried");
                RemoveFromClassList("memoryless");
            }
            else
            {
                RemoveFromClassList("memoried");
                AddToClassList("memoryless");
            }

            //Utility styles
            RemoveFromClassList("utility");
            AddToClassList("no-utility");
            if (node is CompositeNode composite)
            {
                if (composite.useUtility)
                {
                    AddToClassList("utility");
                    RemoveFromClassList("no-utility");
                }
            }
        }

        // Position methods // --------------------------------------------------------------------------//
        
        /// <summary>
        /// Set the view base position
        /// </summary>
        /// <param name="positionBase">View base position.</param>
        public override void SetPosition(Rect positionBase)
        {
            Undo.RecordObject(node, "Behavior Tree (Set Position)");

            node.position.x = positionBase.xMin;
            node.position.y = positionBase.yMin;

            EditorUtility.SetDirty(node);

            this.positionBase = positionBase.position;
            UpdateViewPosition();
        }


        /// <summary>
        /// Set the view base position
        /// </summary>
        /// <param name="positionBase">View base position.</param>
        public void SetPosition(Vector2 positionBase)
        {
            node.position.x = positionBase.x;
            node.position.y = positionBase.y;
            EditorUtility.SetDirty(node);

            this.positionBase = positionBase;
            UpdateViewPosition();
        }

        /// <summary>
        /// Update the view position if any change
        /// </summary>
        private void UpdateViewPosition()
        {
            //Compute new position
            Vector2 position = positionBase + positionOffset;
            Rect newPos = new(position, Vector2.one);

            //Update position
            base.SetPosition(newPos);

            //Sort node children
            SortChildren();
        }

        /// <summary>
        /// Sort node children
        /// </summary>
        public void SortChildren()
        {
            CompositeNode composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPostion);
            }
        }


        /// <summary>
        /// Sort node by horizontal position comparator.
        /// </summary>
        /// <param name="left">Left node</param>
        /// <param name="right">Right node</param>
        /// <returns>-1 if left < right, 1 otherwise </returns>
        private int SortByHorizontalPostion(Node left, Node right)
        {
            if (left.position.x < right.position.x)
            {
                return -1;
            }

            return 1;
        }


        // NodeView configuration // ------------------------------------------------------------------- //
        
        /// <summary>
        /// Configure static style classes
        /// </summary>
        void ConfigureClasses()
        {
            //Node type
            if (node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is RootNode)
            {
                AddToClassList("root");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            

            //Subtree or base node
            if (node is SubtreeNode)
            {
                AddToClassList("subtree-node");
            }
            else
            {
                AddToClassList("base-node");
            }

            //Ghost node
            if (ghost)
            {
                AddToClassList("ghost");
            }
        }

        /// <summary>
        /// Create input ports for the node.
        /// </summary>
        void CreateInputPorts()
        {
            //Create port if not RootNode.
            if(node is not RootNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool)); //Dummy type

                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;

                inputContainer.Add(input);
            }
            
        }

        /// <summary>
        /// Create output port for the node type.
        /// </summary>
        void CreateOutputPorts()
        {
            //Create port for the node type
            if (node is ActionNode)
            {

            }
            else if (node is CompositeNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool)); //Dummy type
            }
            else if (node is DecoratorNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool)); //Dummy type
            }

            //Configure port if any
            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;

                outputContainer.Add(output);
            }
        }

        /// <summary>
        /// Configure port picking.
        /// 
        /// Needed for better port picking.
        /// </summary>
        void ConfigurePortPicking()
        {
            Port[] ports = { input, output };
            string[] names = { "connector", "cap" };

            //Set every port as pickable
            foreach (Port port in ports)
            {
                if (port != null)
                {
                    foreach (string name in names)
                    {
                        VisualElement visualElement = port.Query(name);
                        visualElement.pickingMode = PickingMode.Position;
                    }
                }

            }
        }

        /// <summary>
        /// Bind the label field with the node description.
        /// </summary>
        void ConfigureLabel()
        {
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }
    }
}