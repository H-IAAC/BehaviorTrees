using UnityEngine;
using System.Collections.Generic;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Scriptable object for provide tags
    /// </summary>
    [CreateAssetMenu(menuName = "Behavior Tree/Behavior Tag Container")]
    public class BTagContainer : ScriptableObject, IBTagProvider
    {
        [Tooltip("Tags the container can provide.")] List<BehaviorTag> tags;
        [Tooltip("If should randomize the tags order after providing one.")] public bool randomizeOnProvide;
        [Tooltip("If should randomize the tags order on the enable.")] public bool randomizeOnEnable = false;


        /// <summary>
        /// Get list of tags in the container compatible with agent parameters.
        /// </summary>
        /// <param name="agentParameters">Parameters of the agent.</param>
        /// <returns>Compatible tags</returns>
        public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters)
        {
            List<BehaviorTag> availableTags = new();

            ProvideTags(agentParameters, availableTags);
            return availableTags;
        }

        public void ProvideTags(List<BTagParameter> agentParameters, List<BehaviorTag> availableTags)
        {
            foreach (BehaviorTag tag in tags)
            {
                if (tag.IsCompatible(agentParameters))
                {
                    availableTags.Add(tag);
                }
            }

            if (randomizeOnProvide && availableTags.Count != 0)
            {
                tags.Shuffle();
            }
        }

        public void AddTag(BehaviorTag tag)
        {
            if(tag.container)
            {
                tag.container.RemoveTag(tag);
            }
            
            tag.container = this;

            if(!tags.Contains(tag))
            {
                tags.Add(tag);
            }
        }

        public void RemoveTag(BehaviorTag tag)
        {
            tag.container = null;

            tags.Remove(tag);
        }

        void OnEnable()
        {
            if(randomizeOnEnable)
            {
                tags.Shuffle();
            }
        }

        void OnValidate()
        {
            foreach(BehaviorTag tag in tags)
            {
                tag.container = this;
            }
        }
    }
}