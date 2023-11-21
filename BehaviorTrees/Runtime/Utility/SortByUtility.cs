using System.Collections.Generic;
using UnityEngine;


namespace HIAAC.BehaviorTrees
{
    public static class SortByUtility
    {
        public static List<T> Sort<T>(List<T> list, UtilitySelectionMethod method, float utilityThreshould = 0f) where T : IUseful
        {
            List<T> resultList;

            switch (method)
            {
                case UtilitySelectionMethod.MAXIMUM: //Sort list by utility
                    {
                        resultList = new(list);
                        resultList.Sort(CompareByUtility);
                        break;
                    }

                case UtilitySelectionMethod.WEIGHT_RANDOM:
                    {
                        resultList = new();
                        float weightTotal = 0;
                        List<T> elements = new();

                        //Compute total weight (utility) and add list to list
                        foreach (T node in list)
                        {
                            elements.Add(node);
                            weightTotal += node.GetUtility();
                        }


                        //Sort list by utility 
                        elements.Sort(CompareByUtility);

                        //Create node sequence by utility weight
                        while (elements.Count > 1)
                        {
                            int result;
                            float total = 0;
                            float randVal = Random.Range(0, weightTotal);
                            for (result = 0; result < elements.Count; result++)
                            {
                                total += elements[result].GetUtility();
                                if (total > randVal) break;
                            }


                            T next = elements[result];

                            weightTotal -= next.GetUtility();
                            elements.RemoveAt(result);

                            resultList.Add(next);
                        }

                        resultList.Add(elements[0]);

                        break;
                    }
                case UtilitySelectionMethod.RANDOM_THRESHOULD:
                    {
                        resultList = new(list);

                        //Remove list without minimum utility
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            if (resultList[i].GetUtility() < utilityThreshould)
                            {
                                resultList.RemoveAt(i);
                            }
                        }

                        //Randomize
                        resultList.Shuffle();


                        break;
                    }
                default:
                    resultList = new(list);
                break;
            }

            return resultList;
        }

        /// <summary>
        /// Compare by utility
        /// </summary>
        /// <param name="first">First element to compare</param>
        /// <param name="second">Second element to compare</param>
        /// <returns>1 if first's utility is less than second's, -1 if is greater, 0 if equal. </returns>
        public static int CompareByUtility<T>(T first, T second) where T : IUseful
        {
            if (first.GetUtility() < second.GetUtility())
            {
                return 1;
            }
            else if (first.GetUtility() > second.GetUtility())
            {
                return -1;
            }
            return -0;
        }
    }


}