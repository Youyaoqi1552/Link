using System;
using Common.Extensions;
using Common.UI;
using DG.Tweening;
using UnityEngine;

namespace Game.Anim
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SettingsWindowAnimator : MonoBehaviour, IWindowAnimator
    {
        [SerializeField] private UIWindow window;
        [SerializeField] private RectTransform frameRect;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float distance = 400;
        private Sequence tweener;
        private Action callback;
        
        private void OnDestroy()
        {
            callback = null;
            
            tweener.Kill();
            tweener = null;
        }
        
        public void PlayOpenAnimation(Action callback = null)
        {
            tweener.Kill();
            tweener = null;

            this.callback = callback;
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            frameRect.SetAnchoredPositionX(distance);
            
            window.OnOpenWillStart();

            tweener = DOTween.Sequence();
            tweener.Append(canvasGroup.DOFade(1, 0.15f));
            tweener.Join(frameRect.DOAnchorPosX(0, 0.2f));
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
            tweener.Join(frameRect.DOAnchorPosX(distance, 0.1f));
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
