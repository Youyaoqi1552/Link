using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Effect
{
    public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform attachment;
        [SerializeField] private Vector2 offset = new Vector2(-3, -6);
        [SerializeField] private float duration = 0.1f;

        private Vector2 originalPosition;
        private Tweener tweener;

        private void Awake()
        {
            if (attachment == null)
            {
                attachment = GetComponent<RectTransform>();
            }
            originalPosition = attachment.anchoredPosition;
        }

        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            tweener.Kill();
            tweener = attachment.DOAnchorPos(originalPosition + offset, duration);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            tweener.Kill();
            tweener = attachment.DOAnchorPos(originalPosition, duration);
        }
    }
}
