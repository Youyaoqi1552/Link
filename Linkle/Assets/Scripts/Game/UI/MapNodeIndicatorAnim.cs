using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
    public class MapNodeIndicatorAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform target;

        private Sequence tweener;
        
        private void Awake()
        {
            tweener = DOTween.Sequence();
            tweener.Append(target.DOAnchorPosY(0, 0.5f).SetEase(Ease.InQuad));
            tweener.Insert(0.35f, target.DOScaleY(0.9f, 0.15f));
            tweener.AppendInterval(0.1f);
            tweener.Append(target.DOAnchorPosY(40, 0.8f).SetEase(Ease.OutQuad));
            tweener.Join(target.DOScaleY(1f, 0.12f));
            tweener.SetAutoKill(false);
            tweener.SetLoops(-1);
        }

        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }
    }
}
