using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Link
{
    public class LinkComboAnimPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;

        private Sequence tweener;

        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }

        public void Reset()
        {
            tweener.Kill();
            tweener = null;
            
            amountText.text = string.Empty;
        }
        
        public void ShowCombo(int comboAmount)
        {
            tweener.Kill();

            amountText.text = $"+{comboAmount} Hits";
            amountText.transform.localScale = Vector3.zero;

            tweener = DOTween.Sequence();
            tweener.Append(amountText.rectTransform.DOScale(1, 0.2f));
            tweener.AppendInterval(2);
            tweener.Append(amountText.rectTransform.DOScale(0, 0.21f));
        }
        
        public void StopAnim()
        {
            tweener.Kill();
            tweener = null;
        }
    }
}