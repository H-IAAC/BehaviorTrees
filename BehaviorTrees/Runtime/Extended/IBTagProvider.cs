using System.Collections.Generic;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Interface for objects that provides behavior tags
    /// </summary>
    public interface IBTagProvider
    {
        /// <summary>
        /// Get list of tags compatible with agent parameters.
        /// </summary>
        /// <param name="agentParameters">Parameters of the agent.</param>
        /// <returns>Compatible tags</returns>
        public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters);

        public void ProvideTags(List<BTagParameter> agentParameters, List<BehaviorTag> tags);

        public static void RemoveIncompatibleTags(List<BehaviorTag> tags, List<BTagParameter> minimumValueParameters, List<BTagParameter> maximumValueParameters)
        {
            for(int i = tags.Count-1; i>=0; i--)
            {
                BehaviorTag tag = tags[i];
                if (!BTagParameter.IsCompatible(tag.parameters, minimumValueParameters, maximumValueParameters))
                {
                    tags.RemoveAt(i);
                }
            }
        }

        public static BehaviorTag GetFirstCompatible(List<BehaviorTag> tags, List<BTagParameter> minimumValueParameters, List<BTagParameter> maximumValueParameters)
        {
            for(int i = 0; i<tags.Count; i++)
            {
                BehaviorTag tag = tags[i];
                if (BTagParameter.IsCompatible(tag.parameters, minimumValueParameters, maximumValueParameters))
                {
                    return tags[i];
                }
            }

            return null;
        }
    }
}