using Common.Layout;
using UnityEngine;

namespace Common.UI
{
    public class TabViewController : HorizontalScrollSnap
    {
        [SerializeField] private UITabButton[] tabButtons;
        private int selectedIndex = -1;
        private bool blocking = false;
        
        private void Awake()
        {
            for (var i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].SetContext(this, i);
            }
        }
        
        public void Select(int index, bool animated = false)
        {
            blocking = true;
            selectedIndex = index;
            for (var i = 0; i < tabButtons.Length; i++)
            {
                var tab = tabButtons[i];
                if (selectedIndex == i)
                {
                    tab.Select();
                    GotoPage(i, animated);
                }
                else
                {
                    tab.Deselect();
                }
            }
            blocking = false;
        }

        public void OnTabSelected(int index)
        {
            blocking = true;
            if (selectedIndex >= 0)
            {
                if (selectedIndex != index)
                {
                    tabButtons[selectedIndex].Deselect();
                    
                    selectedIndex = index;
                    tabButtons[selectedIndex].Select();
                    GotoPage(selectedIndex);
                }
                else
                {
                    tabButtons[selectedIndex].RepeatSelect();
                }
            }
            else
            {
                selectedIndex = index;
                tabButtons[selectedIndex].Select();
                GotoPage(selectedIndex);
            }
            blocking = false;
        }
        
        protected override void OnPageChanged(int page)
        {
            base.OnPageChanged(page);

            if (blocking)
            {
                return;
            }
            
            if (selectedIndex >= 0)
            {
                if (selectedIndex != page)
                {
                    tabButtons[selectedIndex].Deselect();
                    
                    selectedIndex = page;
                    tabButtons[selectedIndex].Select();
                }
            }
            else
            {
                selectedIndex = page;
                tabButtons[selectedIndex].Select();
            }
        }
    }
}
