using System;
using Common.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Layout
{
    public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float animDuration = 0.3f;
        [SerializeField] private bool useSwipeDeltaThreshold;
        [SerializeField] private float swipeDeltaThreshold = 5f;
        [SerializeField] private int fastSwipeThreshold = 100;
        
        private float childSize;
        private Vector2 startPosition;
        private int pageCount;
        private int currentPage;
        private Tweener tweener;

        public int CurrentPage => currentPage;

        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }

        private void Start()
        {
            DistributePages();
        }

        [ContextMenu("Distribute Pages")]
        private void DistributePages()
        {
            TryPauseTweener();
            
            if (null == scrollRect)
            {
                scrollRect = GetComponent<ScrollRect>();
            }

            var rectT = scrollRect.GetComponent<RectTransform>();
            var dimension = rectT.rect;
            childSize = dimension.width;
            
            var content = scrollRect.content;
            pageCount = content.childCount;
            
            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.up;
            content.sizeDelta = new Vector2(childSize * pageCount, 0);
            content.anchoredPosition = new Vector2(-childSize * currentPage, 0);
            
            var currentXPosition = 0f;
            for (var i = 0; i < pageCount; i++)
            {
                var child = (RectTransform) content.GetChild(i).GetComponent<RectTransform>();
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.up;
                child.sizeDelta = new Vector2(childSize, 0);
                child.anchoredPosition = new Vector2(currentXPosition, 0f);
                currentXPosition += childSize;
            }
        }

        public void NextPage(bool animated = true)
        {
            if (currentPage + 1 >= pageCount)
            {
                return;
            }
            MoveToPage(currentPage + 1, animated);
        }
        
        public void PreviousPage(bool animated = true)
        {
            if (0 == currentPage)
            {
                return;
            }
            MoveToPage(currentPage - 1, animated);
        }

        public void GotoPage(int page, bool animated = true)
        {
            if (page < 0 || page >= pageCount)
            {
                return;
            }
            MoveToPage(page, animated);
        }

        private void ScrollToClosestPage()
        {
            var anchoredPosition = scrollRect.content.anchoredPosition;
            MoveToPage( (int)Math.Round(-anchoredPosition.x / childSize), true);
        }

        private void MoveToPage(int page, bool animated)
        {
            var oldPage = currentPage;
            currentPage = page;
            var targetPos = -currentPage * childSize;
            if (animated)
            {
                var anchoredPosition = scrollRect.content.anchoredPosition;
                if (tweener == null)
                {
                    tweener = scrollRect.content.DOAnchorPosX(targetPos, animDuration).SetAutoKill(false);
                }
                else
                {
                    tweener.ChangeValues(anchoredPosition, new Vector2(targetPos, anchoredPosition.y), animDuration);
                    tweener.Play();
                }
            }
            else
            {
                TryPauseTweener();
                scrollRect.content.SetAnchoredPositionX(targetPos);
            }

            if (oldPage != currentPage)
            {
                OnPageChanged(currentPage);
            }
        }

        protected virtual void OnPageChanged(int page)
        {
            
        }

        private void TryPauseTweener()
        {
            if (tweener != null && tweener.active && tweener.IsPlaying())
            {
                tweener.Pause();
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            TryPauseTweener();
            startPosition = scrollRect.content.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (useSwipeDeltaThreshold && Mathf.Abs(eventData.delta.x) < swipeDeltaThreshold)
            {
                ScrollToClosestPage();
            }
            else
            {
                var anchoredPosition = scrollRect.content.anchoredPosition;
                var distance = Mathf.Abs(startPosition.x - anchoredPosition.x);
                scrollRect.velocity = Vector3.zero;
                if (distance > fastSwipeThreshold)
                {
                    if (startPosition.x - anchoredPosition.x > 0)
                    {
                        NextPage();
                    }
                    else
                    {
                        PreviousPage();
                    }
                }
                else
                {
                    ScrollToClosestPage();
                }
            }
        }
    }
}
