using TMPro;
using UnityEngine;

namespace Game.Display
{
    public class ItemAmountDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;
        
        private int currentAmount;

        public void SetAmount(int amount)
        {
            currentAmount = amount;
            amountText.text = $"{currentAmount}";
        }
    }
}
