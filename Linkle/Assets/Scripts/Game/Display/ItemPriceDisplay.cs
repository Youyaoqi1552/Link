using TMPro;
using UnityEngine;

namespace Game.Display
{
    public class ItemPriceDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI priceText;
        
        private int currentCost;
        public void SetCost(int cost)
        {
            currentCost = cost;
            priceText.text = $"{currentCost}";
        }
    }
}
