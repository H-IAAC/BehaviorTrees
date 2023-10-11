using System;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    [Serializable]
    public class Optional<T>
    {
        [SerializeField] public bool enabled;
        [SerializeField] public T value;

        public static implicit operator T(Optional<T> optional)
        {
            return optional.value;
        }
    }
}
