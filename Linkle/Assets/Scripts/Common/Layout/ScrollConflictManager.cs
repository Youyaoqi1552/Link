using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Common.Layout
{
    public class ScrollConflictManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [FormerlySerializedAs("m_ParentScrollRect")] [SerializeField] private ScrollRect parentScrollRect;
        
        private ScrollRect selfScrollRect;
        private IBeginDragHandler[] beginDragHandlers;
        private IEndDragHandler[] endDragHandlers;
        private IDragHandler[] dragHandlers;
        private bool scrollOther;
        private bool scrollOtherHorizontally;
        
        private void Start()
        {
            if (parentScrollRect)
            {
                InitialiseConflictManager();
                AssignScrollRectHandlers();
            }
        }

        public void SetParentScrollRect(ScrollRect parentScrollRect)
        {
            this.parentScrollRect = parentScrollRect;
            InitialiseConflictManager();
            AssignScrollRectHandlers();
        }
        
        private void InitialiseConflictManager()
        {
            selfScrollRect = GetComponent<ScrollRect>();
            scrollOtherHorizontally = selfScrollRect.vertical;
            if (scrollOtherHorizontally)
            {
                if (selfScrollRect.horizontal)
                {
                    Debug.LogError("You have added the SecondScrollRect to a scroll view that already has both directions selected");
                }
                if (!parentScrollRect.horizontal)
                {
                    Debug.LogError("The other scroll rect does not support scrolling horizontally");
                }
            }
            else if (!parentScrollRect.vertical)
            {
                Debug.LogError("The other scroll rect does not support scrolling vertically");
            }
        }

        private void AssignScrollRectHandlers()
        {
            beginDragHandlers = parentScrollRect.GetComponents<IBeginDragHandler>();
            dragHandlers = parentScrollRect.GetComponents<IDragHandler>();
            endDragHandlers = parentScrollRect.GetComponents<IEndDragHandler>();
        }

        #region DragHandler

        public void OnBeginDrag(PointerEventData eventData)
        {
            var position = eventData.position;
            var pressPosition = eventData.pressPosition;
            var horizontal = Mathf.Abs(position.x - pressPosition.x);
            var vertical = Mathf.Abs(position.y - pressPosition.y);
            if (scrollOtherHorizontally)
            {
                if (horizontal > vertical)
                {
                    scrollOther = true;
                    selfScrollRect.enabled = false;
                    foreach (var handler in beginDragHandlers)
                    {
                        handler?.OnBeginDrag(eventData);
                    }
                }
            }
            else if (vertical > horizontal)
            {
                scrollOther = true;
                foreach (var handler in beginDragHandlers)
                {
                    handler?.OnBeginDrag(eventData);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                selfScrollRect.enabled = true;
                scrollOther = false;
                foreach (var handler in endDragHandlers)
                {
                    handler?.OnEndDrag(eventData);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (scrollOther)
            {
                foreach (var handler in dragHandlers)
                {
                    handler?.OnDrag(eventData);
                }
            }
        }

        #endregion DragHandler
    }
}