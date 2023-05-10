using DG.Tweening;
using Game.Data;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Link
{
    public class LinkPlayerIndicator : MonoBehaviour
    {
        [SerializeField] private UIAvatar avatarDisplay;
        [SerializeField] private Text remainingPairsText;
        [SerializeField] private Image progressBar;
        
        private float maxWidth;
        private int maxPairs;
        private int remainingPairs;
        private Tweener tweener;

        public void Init(AvatarData avatarData, int maxPairs)
        {
            avatarDisplay.SetAvatar(avatarData);

            this.maxPairs = maxPairs;
            remainingPairs = maxPairs;
            remainingPairsText.text = $"{remainingPairs}";

            progressBar.fillAmount = (maxPairs - remainingPairs) / (float)maxPairs;
        }

        public void SetRemainingPairs(int remainingPairs)
        {
            this.remainingPairs = remainingPairs;
            remainingPairsText.text = $"{remainingPairs}";

            tweener.Kill();
            tweener = progressBar.DOFillAmount((maxPairs - remainingPairs) / (float) maxPairs, 0.2f);
        }
        
        private void OnDestroy()
        {
            tweener.Kill();
            tweener = null;
        }
    }
}