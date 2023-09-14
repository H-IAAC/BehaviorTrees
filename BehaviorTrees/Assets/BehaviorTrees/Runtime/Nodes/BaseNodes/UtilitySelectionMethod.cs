namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Method for composite node sort children by utility
    /// </summary>
    public enum UtilitySelectionMethod
    {
        /// <summary>
        /// Node with bigger utility first.
        /// </summary>
        MAXIMUM,

        /// <summary>
        /// Utility-weighted random order.
        /// </summary>
        WEIGHT_RANDOM,

        /// <summary>
        /// Random order, with minimum threshould.
        /// </summary>
        RANDOM_THRESHOULD
    }
}