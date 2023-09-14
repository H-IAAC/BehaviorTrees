namespace HIAAC.BehaviorTree
{

    /// <summary>
    /// Possible actions to do after executing a behavior tag's tree.
    /// </summary>
    public enum TagLifecycleType
    {
        /// <summary>
        /// Drops the tag.
        /// </summary>
        DROP,

        /// <summary>
        /// Holds the tag (continue using it's tree).
        /// </summary>
        HOLD,

        /// <summary>
        /// Mark the tag as changable if avaiable tags exists.
        /// </summary>
        OVERRIDABLE
    }
}