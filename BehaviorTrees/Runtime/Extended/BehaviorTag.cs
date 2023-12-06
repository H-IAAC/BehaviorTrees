using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HIAAC.BehaviorTrees.Needs;


namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// BehaviorTag contains a tree and parameters to instantiate and 
    /// maintain the tree as a subtree of a RequestBehaviorNode.
    /// </summary>
    /// <remarks>
    /// See "Smart Areas: A Modular Approach to Simulation of Daily Life in an Open World Video Game"
    /// for complete definition.
    /// </remarks>
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tag")]
    public class BehaviorTag : ScriptableObject
    {
        [Tooltip("Tag's tree.")][SerializeField] public BehaviorTree tree;
        [SerializeField] Optional<uint> maxUsers = new(){value=0};

        [Tooltip("Number of users to track if registered or unregistered.")][SerializeField] int maxTrackedUsers = 10;

        [Tooltip("What the node should do when the tree success.")] public TagLifecycleType onSuccess = TagLifecycleType.HOLD;
        [Tooltip("What the node should do when the tree fails.")] public TagLifecycleType onFailure = TagLifecycleType.DROP;
        [Tooltip("What the node should do when the tree returns Running.")] public TagLifecycleType onRunning = TagLifecycleType.HOLD;

        [Tooltip("List of parameters of the tag. Used by the requesting agent.")] public List<BTagParameter> parameters;

        [Tooltip("Minimum parameters the requesting agent should have to use the tag.")] public List<BTagParameter> minimumValueParameters = new();
        [Tooltip("Maximum parameters the requesting agent should have to use the tag.")] public List<BTagParameter> maximumValueParameters = new();

        [SerializeField] public Blackboard blackboard;
        [HideInInspector] public List<bool> passValue;

        [HideInInspector] public BTagContainer container;

        List<GameObject> users = new();
        [HideInInspector] public List<GameObject> newUsers = new();
        [HideInInspector] public List<GameObject> droppedUsers = new();

        [SerializeField] public NeedsContainer advertisedNeeds = new();

        public BehaviorTag()
        {
            blackboard = new(this);
        }

        public BehaviorTree RegisterUser(GameObject user)
        {
            users.Add(user);

            if(newUsers.Count == maxTrackedUsers)
            {
                newUsers.RemoveAt(0);
            }
            newUsers.Add(user);
            
            return tree;
        }

        public void UnregisterUser(GameObject user)
        {
            users.Remove(user);

            if(droppedUsers.Count == maxTrackedUsers)
            {
                droppedUsers.RemoveAt(0);
            }
            droppedUsers.Add(user);
        }

        /// <summary>
        /// Check if tag is compatible with parameters.
        /// </summary>
        /// <param name="parameters">Parameters of the agent.</param>
        /// <returns>True if compatible.</returns>
        public bool IsCompatible(List<BTagParameter> parameters)
        {
            if(maxUsers.enabled && maxUsers <= users.Count)
            {
                return false;
            }

            return BTagParameter.IsCompatible(parameters, minimumValueParameters, maximumValueParameters);
        }

        public void OnValidate()
        {
            if (onRunning == TagLifecycleType.OVERRIDABLE)
            {
                Debug.LogWarning("Cannot override on running. Changing to HOLD");
                onRunning = TagLifecycleType.HOLD;
            }

            if(blackboard == null)
            {
                blackboard = new(this);
                passValue = new();
            }

            if(tree == null)
            {
                return;
            }

            foreach(BlackboardOverridableProperty treeP in tree.blackboard.properties)
            {
                if(!blackboard.HasProperty(treeP.Name) && treeP.Name != "advertisedNeeds")
                {
                    blackboard.CreateProperty(treeP.property.GetType(), treeP.Name);
                    passValue.Add(false);
                }
            }

            for(int i = blackboard.properties.Count-1; i>= 0; i--)
            {
                BlackboardOverridableProperty tagP = blackboard.properties[i];

                if (!tree.blackboard.HasProperty(tagP.Name))
                {
                    blackboard.properties.RemoveAt(i);
                    passValue.RemoveAt(i);
                }
            }

            if (!tree.blackboard.HasProperty("advertisedNeeds"))
            {
                tree.blackboard.CreateProperty(typeof(NeedValueArrayBlackboardProperty), "advertisedNeeds");
                UpdateAdvertisedNeeds();
            }


        }

        public void UpdateAdvertisedNeeds()
        {
            tree.blackboard.SetPropertyValue("advertisedNeeds", advertisedNeeds.needs.ToArray());
        } 

        void OnDisable()
        {
            users.Clear();
        }
    }
}