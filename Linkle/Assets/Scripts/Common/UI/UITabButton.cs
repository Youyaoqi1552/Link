using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI
{
    public abstract class UITabButton : MonoBehaviour, IPointerClickHandler
    {
        protected TabViewController tabController;
        protected int index;

        public void SetContext(TabViewController tabController, int index)
        {
            this.tabController = tabController;
            this.index = index;
        }
        
        public void SetSelect(bool animated = false)
        {
            tabController.Select(index, animated);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            tabController.OnTabSelected(index);
        }

        public abstract void Select();
        
        public abstract void Deselect();

        public virtual void RepeatSelect()
        {
        }
    }
}
