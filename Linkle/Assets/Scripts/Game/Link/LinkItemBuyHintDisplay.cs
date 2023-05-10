using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Link
{
    public class LinkItemBuyHintDisplay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI priceText;

        private Sequence tweener;
        
        public void Dispose()
        {
            tweener.Kill();
            tweener = null;
        }
        
        public void Show(int price)
        {
            tweener.Kill();
            
            priceText.text = $"-{price}";
            
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);

            tweener = DOTween.Sequence();
            tweener.Append(canvasGroup.DOFade(1, 0.3f));
            tweener.AppendInterval(1.5f);
            tweener.Append(canvasGroup.DOFade(0, 0.2f));
            tweener.OnComplete(OnAnimCompleted);
        }

        private void OnAnimCompleted()
        {
            tweener.Kill();
            tweener = null;
            
            gameObject.SetActive(false);
        }


    }
}