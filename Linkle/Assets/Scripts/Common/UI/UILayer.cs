using System.Collections.Generic;
using UnityEngine;

namespace Common.UI
{
    public class UILayer : MonoBehaviour
    {
        [SerializeField] private int baseSortingOrder = 0;
        [SerializeField] private int step = 10;
        
        private List<int> rentedSortingOrders = new List<int>();

        public int RentSortingLayerOrder()
        {
            if (rentedSortingOrders.Count == 0)
            {
                rentedSortingOrders.Add(baseSortingOrder);
                return baseSortingOrder;
            }

            var sortingOrder = rentedSortingOrders[rentedSortingOrders.Count - 1] + step;
            rentedSortingOrders.Add(sortingOrder);
            return sortingOrder;
        }

        public void PutSortingLayerOrder(int order)
        {
            var idx = rentedSortingOrders.IndexOf(order);
            if (idx >= 0)
            {
                rentedSortingOrders.RemoveAt(idx);
            }
        }
    }
}
