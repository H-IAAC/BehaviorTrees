using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public static class IList
    {
        /// <summary>
        /// Randomize the list order.
        /// </summary>
        /// <typeparam name="T">Type of the list</typeparam>
        /// <param name="list">List to randomize</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}