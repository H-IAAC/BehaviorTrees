using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Parameter for a behavior tag or agent/tree.
    /// 
    /// Used for filtering tags.
    /// </summary>
    [Serializable]
    public class BTagParameter
    {
        [Tooltip("Type of the parameter")] public BTagParameterType type;
        [Tooltip("Value the parameter. Must be between 0 and 1.")] [SerializeField] float value = 0;

        /// <summary>
        /// Value the parameter. Must be between 0 and 1.
        /// </summary>
        [SerializeField]
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value < 0 || value > 1)
                {
                    Debug.Log("Value must be in [0, 1].");
                }
                else
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Check if the tag/agent is compatible with some requirements.
        /// </summary>
        /// <param name="parameters">Tag/agent parameters</param>
        /// <param name="minimumValueParameters">Minimum parameter values</param>
        /// <param name="maximumValueParameters">Maximum parameter values. If the agent don't have some parameter is considered 0.</param>
        /// <returns>True if the parameters are compatible.</returns>
        public static bool IsCompatible(List<BTagParameter> parameters, List<BTagParameter> minimumValueParameters, List<BTagParameter> maximumValueParameters)
        {
            //Check miminum constraints
            foreach (BTagParameter minConstraint in minimumValueParameters)
            {
                bool satisfied = false;
                foreach (BTagParameter parameter in parameters)
                {
                    if (parameter.type == minConstraint.type && parameter.Value >= minConstraint.Value)
                    {
                        satisfied = true;
                        break;
                    }
                }

                if (!satisfied)
                {
                    return false;
                }
            }

            //Check maximum constraints
            foreach (BTagParameter parameter in parameters)
            {                
                foreach (BTagParameter maxConstraint in maximumValueParameters)
                {
                    if (parameter.type == maxConstraint.type && parameter.Value > maxConstraint.Value)
                    {
                        return false;
                    }
                }
            }

            //All passed!
            return true;
        }
    }
}