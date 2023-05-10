using Common.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIWindow : MonoBehaviour
    {
        protected int sortingOrder;
        
        public void SetSortingOrder(int sortingOrder)
        {
            this.sortingOrder = sortingOrder;
            
            var canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = sortingOrder;
        }
        
        public virtual void OnCreate(DataBundle context)
        {
        }
        
        public virtual void OnDispose()
        {
        }

        public virtual void OnOpenWillStart()
        {
            
        }
        
        public virtual void OnOpenDidFinish()
        {
            
        }
        
        public virtual void OnCloseWillStart()
        {
            
        }
        
        public virtual void OnCloseDidFinish()
        {
            
        }
        
        public virtual void OnCloseImmediately()
        {
            
        }
        
        public void CloseSelf(bool immediate = false)
        {
            WindowManager.Shared.CloseWindow(this, immediate);
        }
    }
}