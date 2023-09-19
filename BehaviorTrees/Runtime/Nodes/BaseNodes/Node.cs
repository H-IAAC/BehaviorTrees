using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;


namespace HIAAC.BehaviorTrees
{
    
    /// <summary>
    /// Base node class. 
    /// All Behavior Tree nodes inherit from it.
    /// </summary>
    public abstract class Node : ScriptableObject
    {
        [HideInInspector] public NodeState state = NodeState.Runnning; //Current state of the node
        [HideInInspector] public bool started = false; //If node already started

        [HideInInspector] public BehaviorTree tree; //Tree of the node
        [HideInInspector] public Node parent; //Parent node in the tree
        [HideInInspector] public GameObject gameObject; //GameObject the node is binded
        [HideInInspector][SerializeField] public Blackboard blackboard; //Node blackboard

        //View properties
        [HideInInspector] public string guid; //ID of the node
        [HideInInspector] public Vector2 position; //Node position in the view

        //Public and visible
        [SerializeField]
        [SerializeProperty("UseMemory")]
        [Tooltip("If the node should use memory (see node documentation for behavior explanation).")]
        bool useMemory = false;
        
        [TextArea][Tooltip("Description of the node.")] public string description;

        float utility = 0f; //Node current utility value

        /// <summary>
        /// If the node can use memory or not.
        /// </summary>
        public MemoryMode MemoryMode
        {
            private set; get;
        }

        /// <summary>
        /// If the node uses memory or not (have different behavior in different runs).
        /// </summary>
        public bool UseMemory
        {
            get
            {
                return useMemory;
            }

            set
            {
                if (MemoryMode == MemoryMode.Both)
                {
                    useMemory = value;
                }

            }
        }

        /// <summary>
        /// Tranverse the node and all children applying some action.
        /// </summary>
        /// <param name="node">Start node to visit.</param>
        /// <param name="visiter">Action to aply on the nodes.</param>
        public static void Traverse(Node node, Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                List<Node> children = node.GetChildren();

                foreach (Node child in children)
                {
                    Traverse(child, visiter);
                }
            }
        }

        // Constructor and lifecycle // --------------------------------------- //
        // Construct -> Awake -> Start -> Update <-> Start 

        /// <summary>
        /// Node constructor.
        /// </summary>
        /// <param name="memoryMode">MemoryMode specifing if node can use memory.</param>
        public Node(MemoryMode memoryMode = MemoryMode.Memoryless)
        {
            //Configure memory mode
            MemoryMode = memoryMode;
            switch (memoryMode)
            {
                case MemoryMode.Memoried:
                    this.useMemory = true;
                    break;
                case MemoryMode.Memoryless:
                    this.useMemory = false;
                    break;
                case MemoryMode.Both:
                    break;
            }

            //Create GUID
            guid = Guid.NewGuid().ToString();

            //Create blackboard
            blackboard = new(this);
        }

        /// <summary>
        /// Create new GUID at each Awake.
        /// 
        /// Avoid problems with multiple trees in same view (ghost trees)
        /// and duplicating trees.
        /// </summary>
        void Awake()
        {
            guid = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Compute the utility and start the node
        /// </summary>
        public void Start()
        {
            ComputeUtility();
            OnStart();
        }

        /// <summary>
        /// Update the node.
        /// 
        /// Also starts if it hasn't already been.
        /// </summary>
        /// <returns>Current node state</returns>
        public NodeState Update()
        {
            //Start
            if (!started)
            {
                Start();
                started = true;
            }

            //Update
            state = OnUpdate();

            //Stop if end state
            if (state == NodeState.Failure || state == NodeState.Success)
            {
                OnStop();
                started = false;
            }

            return state;
        }

        // Clone // ----------------------------------------------------------- //

        /// <summary>
        /// Criate a clone of the node.
        /// </summary>
        /// <returns>Clone of the node.</returns>
        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.guid = guid;
            return clone;
        }

        // Utility // --------------------------------------------------------- //

        /// <summary>
        /// Get current utility value.
        /// </summary>
        /// <returns>Utility value.</returns>
        public float GetUtility()
        {
            return utility;
        }

        /// <summary>
        /// Updates the utility value.
        /// </summary>
        public void ComputeUtility()
        {
            utility = OnComputeUtility();
        }

        // Properties // ------------------------------------------------------ //

        /// <summary>
        /// Create new property in the node blackboard.
        /// </summary>
        /// <param name="type">Type of the property (must inherit BlackboardProperty)</param>
        /// <param name="name">Name of the property</param>
        /// <returns>Created property.</returns>
        public BlackboardProperty CreateProperty(Type type, string name)
        {
            return blackboard.CreateProperty(type, name);
        }

        /// <summary>
        /// Get the current value of a property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent (tree) blackboard.</param>
        /// <returns>Value of the property.</returns>
        public object GetPropertyValue(string name, bool forceNodeProperty = false)
        {
            return blackboard.GetPropertyValue(name, forceNodeProperty);
        }

        /// <summary>
        /// Get the current value of a property.
        /// </summary>
        /// <typeparam name="T">Type of the property to cast when returning.</typeparam>
        /// <param name="name">Name of the property</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent (tree) blackboard if mapped.</param>
        /// <returns>Value of the property.</returns>
        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {
            return blackboard.GetPropertyValue<T>(name, forceNodeProperty);
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">New value.</param>
        /// <param name="forceNodeProperty">If true, don't uses the parent (tree) blackboard if mapped.</param>
        public void SetPropertyValue<T>(string name, T value, bool forceNodeProperty = false)
        {
            blackboard.SetPropertyValue(name, value, forceNodeProperty);
        }

        /// <summary>
        /// Deletes all node's properties.
        /// </summary>
        /// <param name="dontDelete">List of propertie's names to not delete.</param>
        public void ClearPropertyDefinitions(List<string> dontDelete = null)
        {
            blackboard.ClearPropertyDefinitions(dontDelete);
        }

        /// <summary>
        /// Checks if node have property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if node have property with given name.</returns>
        public bool HasProperty(string name)
        {
            return blackboard.HasProperty(name);
        }


        // To override // ----------------------------------------------------- //
        
        /// <summary>
        /// Called when node starts
        /// </summary>
        public abstract void OnStart();
        
        /// <summary>
        /// Called when node stops (when Success or Failure)
        /// </summary>
        public abstract void OnStop();

        /// <summary>
        /// Callend on node update (every time the node is visited in the tree).
        /// </summary>
        /// <returns>The new node state.</returns>
        public abstract NodeState OnUpdate();
        
        /// <summary>
        /// Add child to the node.
        /// 
        /// IMPORTANT: should also set the child parent to this node.
        /// </summary>
        /// <param name="child">Child to add to the node.</param>
        public abstract void AddChild(Node child);

        /// <summary>
        /// Remove child from the node.
        /// 
        /// IMPORTANT: should also set the child parent to null.
        /// </summary>
        /// <param name="child">Child to remove from node</param>
        public abstract void RemoveChild(Node child); //Must set child parent

        /// <summary>
        /// Get list of all children of the node.
        /// </summary>
        /// <returns>List with node children.</returns>
        public abstract List<Node> GetChildren();

        /// <summary>
        /// Called when the node should update its utility value.
        /// </summary>
        /// <returns>New utility value.</returns>
        protected virtual float OnComputeUtility()
        {
            return 0f;
        }
    }
}