using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// View for BehaviorTree Editor
    /// </summary>
    public class BehaviorTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public Action<NodeView> OnNodeSelected; //Action to execute when node is selected

        public BehaviorTree tree; //Active behavior tree asset

        public BlackboardView blackboard; //Blackboard view

        List<GraphElement> clipboard; //Elements copied to clipboard
        Vector2 mousePositionContextMenu = Vector2.zero; //Mouse position when creating context menu

        List<BehaviorTree> ghostTrees; //List of trees beeing visualized in ghost mode
        Dictionary<SubtreeNode, BehaviorTree> subtreeNodesVisible; //Mapping of subtree nodes and subtrees beeing visualized

        GridBackground grid; //Background

        /// <summary>
        /// View constructor
        /// </summary>
        public BehaviorTreeView()
        {
            grid = new();
            Insert(0, grid);

            this.AddManipulator(new ContentZoomer()); //Zoom view
            this.AddManipulator(new ContentDragger()); //Move things around
            this.AddManipulator(new BTViewDoubleClick(this)); //Double click
            this.AddManipulator(new SelectionDragger()); //Select things
            this.AddManipulator(new RectangleSelector()); //Select multiple things

            //Get and assign view USS style
            string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            styleSheets.Add(styleSheet);

            //Subscribe undo redo callback
            Undo.undoRedoPerformed += OnUndoRedo;

            //Configure duplicate objects and callbacks
            clipboard = new();
            serializeGraphElements += OnCopyCut;
            unserializeAndPaste += OnPasteDuplicate;

            //Create subtree view collections
            ghostTrees = new();
            subtreeNodesVisible = new();
        }

        /// <summary>
        /// Repopulate view and save asset on Undo/Redo
        /// </summary>
        void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Get NodeView from node
        /// </summary>
        /// <param name="node">Node to get view</param>
        /// <returns>Corresponding node NodeView</returns>
        public NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        /// <summary>
        /// Populate view with new tree
        /// </summary>
        /// <param name="tree">BehaviorTree to populate view</param>
        public void PopulateView(BehaviorTree tree)
        {
            //Clear view

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            //clipboard.Clear();
            ghostTrees.Clear();

            //Update selected tree
            this.tree = tree;
            if (tree == null)
            {
                return;
            }

            //Create root node
            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            //Create node views
            foreach (Node node in tree.nodes)
            {
                CreateNodeView(node);
            }

            //Connect node views (create edges)
            foreach (Node node in tree.nodes)
            {
                NodeView parentView = FindNodeView(node);

                List<Node> children = node.GetChildren();
                foreach (Node child in children)
                {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }
        }

        /// <summary>
        /// Create node view from node.
        /// 
        /// Runtime view mode is setted from active tree
        /// </summary>
        /// <param name="node">Node to create view.</param>
        /// <returns>Created view.</returns>
        NodeView CreateNodeView(Node node)
        {
            return CreateNodeView(node, tree.Runtime);
        }


        /// <summary>
        /// Create node view from node.
        /// </summary>
        /// <param name="node">Node to create view.</param>
        /// <param name="runtime">Runtime state of the view.</param>
        /// <param name="ghost">Ghost state of the view.</param>
        /// <returns>Created view.</returns>
        NodeView CreateNodeView(Node node, bool runtime, bool ghost = false)
        {
            NodeView nodeView = new(node, runtime, ghost)
            {
                OnNodeSelected = OnNodeSelected
            };
            AddElement(nodeView);

            return nodeView;
        }


        /// <summary>
        /// Get compatible ports from some start port
        /// 
        /// Compatible port:
        /// - Not connecting with itself 
        /// - Not connecting input with input
        /// - Not connecting output with output
        /// 
        /// </summary>
        /// <param name="startPort">Start port</param>
        /// <param name="nodeAdapter">Not used</param>
        /// <returns>List of compatible port</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //                           It's not input-input connection            It's not connected to itself
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        /// <summary>
        /// Update node view state in editor frame
        /// </summary>
        public void UpdateNodeStates()
        {
            foreach (var node in nodes)
            {
                NodeView view = node as NodeView;
                view.UpdateState();
            }
        }

        // Creating new nodes // -----------------------------------------------------------------

        /// <summary>
        /// Create contextual menu with node types
        /// </summary>
        /// <param name="evt">Event to add menu actions</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            //Basic node types
            Type[] baseTypes = { typeof(ActionNode), typeof(CompositeNode), typeof(DecoratorNode) };

            foreach (Type baseType in baseTypes)
            {   
                //Get all derived node types
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);
                
                //Add action for create new node of type
                foreach (Type type in types)
                {
                    if(type == typeof(RootNode))
                    {
                        continue;
                    }
                    
                    evt.menu.AppendAction($"{baseType.Name}/{type.Name}", (a) => CreateNode(type));
                }
            }

            //Save mouse position for setting new node position
            mousePositionContextMenu = evt.localMousePosition;

        }

        /// <summary>
        /// Create new node from type
        /// 
        /// Node position is setted from previous contextual menu position
        /// </summary>
        /// <param name="type">Type to create node. Must inherit from Node.</param>
        void CreateNode(Type type)
        {
            if (!tree)
            {
                Debug.LogError("Cannot create node without active tree asset.");
                return;
            }

            Node node = tree.CreateNode(type);
            NodeView view = CreateNodeView(node);

            Vector2 position = grid.ChangeCoordinatesTo(contentViewContainer, mousePositionContextMenu);
            view.SetPosition(position);
        }


        // Process GraphView change // --------------------------------------------------------------

        /// <summary>
        /// Process GraphViewChange
        /// </summary>
        /// <param name="graphViewChange">Change that occurred</param>
        /// <returns>Change that occurred</returns>
        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                RemoveElements(graphViewChange.elementsToRemove);
            }

            if (graphViewChange.edgesToCreate != null)
            {
                CreateEdges(graphViewChange.edgesToCreate);
            }

            if (graphViewChange.movedElements != null)
            {
                OnMoveElements();
            }


            return graphViewChange;
        }

        /// <summary>
        /// Remove elements
        /// </summary>
        /// <param name="elementsToRemove">Elements to remove</param>
        void RemoveElements(List<GraphElement> elementsToRemove)
        {
            foreach (GraphElement elem in elementsToRemove)
            {
                if (elem is NodeView nodeView && nodeView.Ghost == false)
                {
                    //NodeView -> remove subtree if showing and delete node

                    if (nodeView.node is SubtreeNode subtreeNode)
                    {
                        ToggleSubtreeView(subtreeNode, forceHide:true);
                    }

                    tree.DeleteNode(nodeView.node);
                }
                else if (elem is Edge edge)
                {
                    //Edge -> remove child from parent 

                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    Undo.RecordObject(parentView.node, "Behavior Tree (RemoveChild)");
                    parentView.node.RemoveChild(childView.node);
                    EditorUtility.SetDirty(parentView.node);
                }
                else if (elem is BlackboardField field)
                {
                    //BlackboardField -> delegate to view to delete

                    blackboard.DeleteProperty(field);
                }
            }
        }

        /// <summary>
        /// Handle new created edges
        /// </summary>
        /// <param name="edgesToCreate">Create edges</param>
        void CreateEdges(List<Edge> edgesToCreate)
        {
            foreach (Edge edge in edgesToCreate)
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                Undo.RecordObject(parentView.node, "Behavior Tree (AddChild)");
                parentView.node.AddChild(childView.node);
                parentView.SortChildren();
                EditorUtility.SetDirty(parentView.node);
            }
        }

        /// <summary>
        /// Update view when some node is moved
        /// </summary>
        void OnMoveElements()
        {
            foreach (var node in nodes)
            {
                NodeView view = node as NodeView;

                //Sort children for composite behavior
                view.SortChildren();

                // Update ghost tree position if SubtreeNode
                if (view.node is SubtreeNode subtreeNode)
                {
                    UpdateGhostTree(subtreeNode);
                }
            }
        }

        // Subtree view //--------------------------------------------------------------------------- //

        /// <summary>
        /// Activate/Deactivate subtree view.
        /// </summary>
        /// <param name="subtreeNode">Node to toggle subtree state.</param>
        /// <param name="forceHide">If true, only hide subtree if showing, not show.</param>
        public void ToggleSubtreeView(SubtreeNode subtreeNode, bool forceHide = false)
        {
            if (subtreeNodesVisible.ContainsKey(subtreeNode)) //Already showing this node subtree (even if old changed tree)
            {
                BehaviorTree tree = subtreeNodesVisible[subtreeNode];

                if (ghostTrees.Contains(tree))
                {
                    RemoveGhostTree(tree);
                }

                subtreeNodesVisible.Remove(subtreeNode);
            }
            else if(forceHide) //Not showing and shouldn't show
            {
                return;
            }
            else //Not showing -> create new ghost view
            {  
                //Trees to check and show. Priority to runtime.
                BehaviorTree runtimeTree = subtreeNode.RuntimeTree;
                BehaviorTree staticTree = subtreeNode.Subtree;
                
                BehaviorTree[] trees = new[] { runtimeTree, staticTree };
                foreach (BehaviorTree tree in trees)
                {
                    if (tree == null || tree == this.tree) //Can't show null tree or the active tree.
                    {
                        continue;
                    }
    
                    if (ghostTrees.Contains(tree)) //Showing tree, but for another node -> Stop showing
                    {
                        RemoveGhostTree(tree);

                        SubtreeNode key = subtreeNodesVisible.FirstOrDefault(x => x.Value == tree).Key;
                        subtreeNodesVisible.Remove(key);
                    }

                    //Have tree and is not the editor tree -> Show
                    AddGhostTree(tree, subtreeNode);
                    subtreeNodesVisible.Add(subtreeNode, tree);

                    break;
                }
            }
        }
        
        /// <summary>
        /// Remove ghost tree (subtree) view.
        /// </summary>
        /// <param name="ghostTree">Ghost tree to remove.</param>
        void RemoveGhostTree(BehaviorTree ghostTree)
        {
            //Remove nodeViews of ghost tree nodes
            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = FindNodeView(node);

                if (view.output != null)
                {
                    foreach (Edge edge in view.output.connections)
                    {
                        RemoveElement(edge);
                    }

                }

                if(node is SubtreeNode subtreeNode)
                {
                    ToggleSubtreeView(subtreeNode, forceHide: true);
                }

                RemoveElement(view);
            }

            //Remove ghost tree of showing ghost trees
            ghostTrees.Remove(ghostTree);
            return;
        }

        /// <summary>
        /// Compute offset for positioning ghost tree nodes. 
        /// </summary>
        /// <param name="subtreeNode">SubtreeNode of the ghost tree.</param>
        /// <returns>Offset to subtree nodes.</returns>
        Vector2 ComputeGhostViewOffset(SubtreeNode subtreeNode)
        {
            Vector2 offset = subtreeNode.position + //Base SubtreeNode position 
                            new Vector2(0, 100) - //Spacing between node and subtree
                            subtreeNode.Subtree.rootNode.position +  //Remove subtree root node offset
                            FindNodeView(subtreeNode).PositionOffset; //Offset of SubtreeNode view

            return offset;
        }

        /// <summary>
        /// Create the view of some ghost tree.
        /// </summary>
        /// <param name="ghostTree">GhostTree to create view.</param>
        /// <param name="subtreeNode">SubtreeNode of ghost tree.</param>
        void AddGhostTree(BehaviorTree ghostTree, SubtreeNode subtreeNode)
        {
            
            //Create views
            Vector2 offset = ComputeGhostViewOffset(subtreeNode);
            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = CreateNodeView(node, ghostTree.Runtime, true);
                view.PositionOffset = offset;
            }

            //Connect edges
            foreach (Node node in ghostTree.nodes)
            {
                NodeView parentView = FindNodeView(node);

                List<Node> children = node.GetChildren();
                foreach (Node child in children)
                {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    edge.capabilities = 0;
                    edge.pickingMode = PickingMode.Ignore;
                    AddElement(edge);
                }
            }

            ghostTrees.Add(ghostTree);
        }

        /// <summary>
        /// Update ghost tree view nodes position.
        /// </summary>
        /// <param name="subtreeNode">SubtreeNode to update subtree.</param>
        void UpdateGhostTree(SubtreeNode subtreeNode)
        {   
            //Check if is showing subtree
            if(!subtreeNodesVisible.ContainsKey(subtreeNode))
            {
                return;
            }

            BehaviorTree ghostTree = subtreeNodesVisible[subtreeNode];
            
            //Update nodes positions.
            Vector2 offset = ComputeGhostViewOffset(subtreeNode);
            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = FindNodeView(node);

                view.PositionOffset = offset;
            }
        }

        // Duplicate node functions // ------------------------------------------------------------- //

        /// <summary>
        /// Callback for copy elements.
        /// 
        /// Clear clipboard and insert new elements.
        /// </summary>
        /// <param name="elements">Elements beeing copied</param>
        /// <returns>Empty string</returns>
        string OnCopyCut(IEnumerable<GraphElement> elements)
        {
            clipboard.Clear();
            clipboard.AddRange(elements);
            return "";
        }

        /// <summary>
        /// Callback for pasting and duplicating operations
        /// </summary>
        /// <param name="operationName">Name of operation</param>
        /// <param name="data">Not used</param>
        void OnPasteDuplicate(string operationName, string data)
        {
            if (operationName == "Duplicate")
            {
                //Stop selecting nodes for select new created nodes
                ClearSelection();

                //Clone nodes
                Dictionary<Node, Node> originalToClone = new();
                foreach (GraphElement element in clipboard)
                {
                    if (element is NodeView view)
                    {
                        Node node = view.node;
                        Node clone = tree.DuplicateNode(node);

                        clone.position.x += 10;

                        NodeView cloneView = CreateNodeView(clone);

                        AddToSelection(cloneView);

                        originalToClone.Add(node, clone);
                    }
                }

                //Create connections if duplicating multiple nodes and originals were connected
                foreach (Node node in originalToClone.Keys)
                {
                    if (node.parent == null)
                    {
                        continue;
                    }

                    if (originalToClone.ContainsKey(node.parent))
                    {
                        Node parentClone = originalToClone[node.parent];
                        Node clone = originalToClone[node];

                        parentClone.AddChild(clone);

                        NodeView parentView = FindNodeView(parentClone);
                        NodeView childView = FindNodeView(clone);

                        Edge edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                }
            }

        }
    }
}