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
        [Tooltip("Tags the container can provide.")] public List<BehaviorTag> tags;
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

            return availableTags;
        }

        void OnEnable()
        {
            if(randomizeOnEnable)
            {
                tags.Shuffle();
            }
        }
    }
}