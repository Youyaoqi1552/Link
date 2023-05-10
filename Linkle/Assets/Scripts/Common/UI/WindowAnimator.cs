using System;
using DG.Tweening;
using UnityEngine;

namespace Common.UI
{
    public interface IWindowAnimator
    {
        void PlayOpenAnimation(Action callback = null);

        void PlayCloseAnimation(Action callback = null);
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowAnimator : MonoBehaviour, IWindowAnimator
    {
        private UIWindow window;
        private RectTransform targetRect;
        private RectTransform frameRect;
        private CanvasGroup canvasGroup;
        private Sequence tweener;
        private Action callback;
        
        private void Awake()
        {
            window = GetComponent<UIWindow>();
            targetRect = window.GetComponent<RectTransform>();
            canvasGroup = window.GetComponent<CanvasGroup>();

            var frame = targetRect.Find("Frame");
            if (frame == null)
            {
                frameRect = targetRect;
            }
            else
            {
                frameRect = frame.GetComponent<RectTransform>();
            }
        }

        private void OnDestroy()
        {
            callback = null;
            
            tweener.Kill();
            tweener = null;

            frameRect = null;
            canvasGroup = null;
            targetRect = null;
            window = null;
        }

        public void PlayOpenAnimation(Action callback = null)
        {
            tweener.Kill();
            tweener = null;

            this.callback = callback;
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            frameRect.localScale = Vector3.zero;
            
            window.OnOpenWillStart();

            tweener = DOTween.Sequence();
            tweener.Append(canvasGroup.DOFade(1, 0.15f));
            tweener.Join(frameRect.DOScale(1.2f, 0.2f));
            tweener.Append(frameRect.DOScale(1f, 0.1f));
            tweener.OnComplete(OnOpenAnimCompleted);
        }

        public void PlayCloseAnimation(Action callback = null)
        {
            tweener.Kill();
            tweener = null;
            
            this.callback = callback;
            canvasGroup.interactable = false;
            
            window.OnCloseWillStart();
            
            tweener = DOTween.Sequence();
            tweener.Append(canvasGroup.DOFade(0, 0.1f));
            tweener.Join(frameRect.DOScale(0f, 0.1f));
            tweener.OnComplete(OnCloseAnimCompleted);
        }

        private void OnOpenAnimCompleted()
        {
            canvasGroup.interactable = true;
            
            window.OnOpenDidFinish();
            callback?.Invoke();
        }
        
        private void OnCloseAnimCompleted()
        {
            window.OnCloseDidFinish();
            callback?.Invoke();
        }
    }
}