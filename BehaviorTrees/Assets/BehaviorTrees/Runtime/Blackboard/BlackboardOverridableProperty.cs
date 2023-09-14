using System;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Wrapper class for BlackboardProperty.
    /// 
    /// Adds the parent name for mapping the property to some parent blackboard property.
    /// </summary>
    [Serializable]
    public class BlackboardOverridableProperty
    {
        /// <summary>
        /// Property itself.
        /// </summary>
        [SerializeReference] public BlackboardProperty property;

        /// <summary>
        /// Name for mapping to parent blackboard.
        /// </summary>
        public string parentName = "";

        /// <summary>
        /// Property name.
        /// </summary>
        public string Name
        {
            get
            {
                return property.PropertyName;
            }
            set
            {
                property.PropertyName = value;
            }
        }
    }
}