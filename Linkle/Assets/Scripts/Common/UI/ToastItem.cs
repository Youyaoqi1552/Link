using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Common.UI
{
    public class ToastItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private CanvasGroup canvasGroup;

        private Sequence tweener;
        
        public void ShowWithMessage(string message, float duration)
        {
            tweener.Kill();
            tweener = null;
            
            messageText.text = message;
            canvasGroup.alpha = 0;

            var distance = 150;
            
            var rect = GetComponent<RectTransform>();
            var anchoredPos = rect.anchoredPosition;

            anchoredPos.y -= distance;
            rect.anchoredPosition = anchoredPos;

            tweener = DOTween.Sequence();
            tweener.Append(canvasGroup.DOFade(1, 0.15f));
            anchoredPos.y += distance;
            tweener.Join(rect.DOAnchorPosY(anchoredPos.y, 0.2f));
            
            tweener.AppendInterval(duration);
            
            tweener.Append(canvasGroup.DOFade(0, 0.15f));
            anchoredPos.y += distance;
            tweener.Join(rect.DOAnchorPosY(anchoredPos.y, 0.15f));
            tweener.OnComplete(DestroySelf);
        }

        private void DestroySelf()
        {
            GameObject.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }
    }
}
