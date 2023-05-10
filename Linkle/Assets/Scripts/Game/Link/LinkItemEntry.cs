using System;
using Game.Define;
using Game.Display;
using Game.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Link
{
    public class LinkItemEntry : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemAmountDisplay amountDisplay;
        [SerializeField] private ItemPriceDisplay priceDisplay;

        public ItemType ItemType => itemType;
        
        public event Action<LinkItemEntry> OnPressed;
        
        public void Init()
        {
            Refresh();
        }

        public void Refresh()
        {
            var amount = UserManager.GetItemAmount(itemType); 
            amountDisplay.SetAmount(amount);
            priceDisplay.SetCost(Global.GetItemPrice(itemType));
            
            if (amount > 0)
            {
                priceDisplay.gameObject.SetActive(false);
                amountDisplay.gameObject.SetActive(true);
            }
            else
            {
                amountDisplay.gameObject.SetActive(false);
                priceDisplay.gameObject.SetActive(true);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPressed?.Invoke(this);
        }
    }
}
