using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Main Behavior Tree class.
    /// </summary>
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tree")]
    public class BehaviorTree : ScriptableObject
    {
        NodeState treeState = NodeState.Runnning; //Current tree state (same as root)

        [SerializeField] public RootNode rootNode; //Root node 
        [SerializeField] public List<Node> nodes = new(); //All nodes

        [SerializeField] public List<BTagParameter> bTagParameters = new(); //BehaviorTags of the tree.

        bool runtime = false; //If the tree is runtime (is binded to object and can be runned).

        [SerializeField] public Blackboard blackboard;

        /// <summary>
        /// If the tree is runtime (is binded to object and can be runned).
        /// </summary>
        public bool Runtime
        {
            get
            {
                return runtime;
            }
        }

        // Lifecyle // ----------------------------------------------------------------------------------- //

        public BehaviorTree()
        {
            blackboard = new(this);
        }

        /// <summary>
        /// Binds the tree to the game object. 
        /// All nodes will have the refence to the object.
        /// </summary>
        /// <param name="gameObject">GameObject to bind the tree.</param>
        public void Bind(GameObject gameObject)
        {
            runtime = true;
            Node.Traverse(rootNode, (node) =>
                {
                    node.gameObject = gameObject;
                    node.blackboard.parent = blackboard;
                }
            );
        }

        /// <summary>
        /// Start the tree.
        /// 
        /// Is automatically done when calling Update for the first time.
        /// </summary>
        /// <exception cref="Exception">If the tree is not runtime (can't be run).</exception>
        public void Start()
        {
            if (!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }

            if (!rootNode.started)
            {
                rootNode.Start();
            }
        }

        /// <summary>
        /// Updates the tree, tranversing it's nodes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If the tree is not runtime (can't be run).</exception>
        public NodeState Update()
        {
            if (!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }

            if (rootNode.state == NodeState.Runnning)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        /// <summary>
        /// Get the tree utility value (same as the root node).
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If the tree is not runtime (doesn't have utility value).</exception>
        public float GetUtility()
        {
            if (!runtime)
            {
                throw new Exception("Trying to run non-runtime tree");
            }

            return rootNode.GetUtility();
        }

        // Tree manipulation // -------------------------------------------------------------------------- //

        /// <summary>
        /// Creates a clone of the tree, with same nodes and properties.
        /// </summary>
        /// <returns>Cloned tree.</returns>
        public BehaviorTree Clone()
        {
            //Create new tree and clone nodes.
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone() as RootNode;

            //Add nodes to tree and create new-original map.
            tree.nodes = new List<Node>();
            List<string> clonedGUID = new();
            Node.Traverse(tree.rootNode, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });

            //Check for nodes that aren't in main tree (not child of root) and clone.
            foreach (Node origNode in this.nodes)
            {
                if (!clonedGUID.Contains(origNode.guid)) //Node not cloned
                {
                    //Get root node of other tree
                    Node parent = origNode;
                    while (parent.parent != null)
                    {
                        parent = parent.parent;
                    }

                    //Clone nodes and add to the tree
                    Node cloned = parent.Clone();
                    Node.Traverse(cloned, (node) => { tree.nodes.Add(node); clonedGUID.Add(node.guid); });
                }
            }

            return tree;
        }

        /// <summary>
        /// Validate tree elements, searching for inconsistencies.
        /// 
        /// Used when opening the asset.
        /// </summary>
        public void Validate()
        {
            //Remove null elements
            nodes.RemoveAll(item => item == null);
            blackboard.properties.RemoveAll(item => item == null || item.property == null);

            //Force node.tree = this and set parent blackboard
            nodes.ForEach(x => { x.tree = this; x.blackboard.parent = blackboard; } );
        }

        // Properties // --------------------------------------------------------------------------------- //
        

        /// <summary>
        /// Create new BlackboardProperty.
        /// </summary>
        /// <param name="type">Type of property. Must inherit BlackboardProperty.</param>
        /// <returns>Created property</returns>
        public BlackboardProperty CreateProperty(Type type)
        {
            return blackboard.CreateProperty(type);
        }

        /// <summary>
        /// Deletes BlackboardProperty of the tree.
        /// </summary>
        /// <param name="property">Property to delete.</param>
        public void DeleteProperty(BlackboardProperty property)
        {
            blackboard.DeleteProperty(property);
        }

        public void SetPropertyValue<T>(string name, T value)
        {
            name = GetType().Name + "/" + name;
            blackboard.GetProperty(name).Value = value;
        }

        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {
            name = GetType().Name + "/" + name;
            return blackboard.GetPropertyValue<T>(name, forceNodeProperty);
        }
        


        // Nodes // -------------------------------------------------------------------------------------- //

        /// <summary>
        /// Creates new node in the tree.
        /// </summary>
        /// <param name="type">Type of node to create. Must inherit Node.</param>
        /// <returns>Created node.</returns>
        public Node CreateNode(Type type)
        {
            //Creates and configures the node.
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.tree = this;
            node.blackboard.parent = blackboard;

            //Record adding the node
#if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (CreateNode)");
#endif

            nodes.Add(node);

            //Save node to asset
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");

            AssetDatabase.SaveAssets();
#endif

            //Save changes
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif

            return node;
        }

        /// <summary>
        /// Duplicates tree node.
        /// 
        /// Node subclass specific properties may not be cloned. 
        /// </summary>
        /// <param name="node">Node to duplicate.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If node isn't in the tree.</exception>
        public Node DuplicateNode(Node node)
        {
            //Check if node is in the tree
            if (!nodes.Contains(node))
            {
                throw new ArgumentException("Node to duplicate is not in tree.");
            }

            //Clone node and basic properties
            Node clone = CreateNode(node.GetType());
            clone.position = node.position;
            clone.position.x += 10;
            clone.UseMemory = node.UseMemory;
            clone.description = node.description;

            //Clone blackboard values
            for(int i = 0; i<clone.blackboard.properties.Count; i++)
            {
                BlackboardOverridableProperty originalP = node.blackboard.properties[i];
                BlackboardOverridableProperty cloneP = clone.blackboard.properties[i];

                cloneP.property.Value = originalP.property.Value;
                cloneP.property.PropertyName = originalP.property.PropertyName;
                cloneP.parentName = originalP.parentName;
            }

            //Clone useUtility if composite.
            if (clone is CompositeNode compositeClone)
            {
                CompositeNode composite = node as CompositeNode;
                compositeClone.useUtility = composite.useUtility;
            }

            return clone;
        }

        /// <summary>
        /// Deletes node from tree;
        /// </summary>
        /// <param name="node">Node to delete</param>
        public void DeleteNode(Node node)
        {
            //Record delete action
#if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (DeleteNode)");
#endif

            //Delete node
            nodes.Remove(node);
            node.ClearPropertyDefinitions();

            //Destroy asset
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
#endif
        }
        
    }
}