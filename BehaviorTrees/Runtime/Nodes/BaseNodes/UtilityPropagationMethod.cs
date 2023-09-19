namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Method to propagate utility for composite nodes
    /// </summary>
    public enum UtilityPropagationMethod
    {
        /// <summary>
        /// Biggest utility of all children.
        /// </summary>
        MAXIMUM,

        /// <summary>
        /// Smallest utility of all children.
        /// </summary>
        MINIMUM,

        /// <summary>
        /// Probability of all nodes successing, child utility is probability of success.
        /// Π child_utility
        /// </summary>
        ALL_SUCESS_PROBABILITY,

        /// <summary>
        /// Probability at least one child successing, child utility is probability of success.
        /// 1 - Π (1-child_utility) 
        /// </summary>
        AT_LEAST_ONE_SUCESS_PROBABILITY,

        /// <summary>
        /// Sum of children utility.
        /// </summary>
        SUM,

        /// <summary>
        /// Average of children utility.
        /// </summary>
        AVERAGE
    }
}