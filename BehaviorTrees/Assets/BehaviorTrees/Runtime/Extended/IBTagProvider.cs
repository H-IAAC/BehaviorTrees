using System.Collections.Generic;

namespace HIAAC.BehaviorTree
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
    }
}