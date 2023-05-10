using Common.Utility;
using Game.Event;
using Game.Manager;
using TMPro;
using UnityEngine;

namespace Game.Display
{
    public class LifeDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI infoText;
        
        private int lifeCount = -1;
        private long remainingTimeInSeconds;
        
        public void Init()
        {
            lifeCount = UserManager.PlayerData.lives;
            amountText.text = lifeCount.ToString();
            
            if (lifeCount == Global.GameSettingsConfig.maxLives)
            {
                infoText.text = "FULL";
            }
            else
            {
                remainingTimeInSeconds = UserManager.GetLifeRestoringRemainingTime();
                infoText.text = DateTimeUtil.Format(remainingTimeInSeconds);
            }
            
            PlayerEvent.LifeRestoring += OnLifeRestoring;
            PlayerEvent.LivesChanged += OnLivesChanged;
        }
        
        public void Dispose()
        {
            PlayerEvent.LifeRestoring -= OnLifeRestoring;
            PlayerEvent.LivesChanged -= OnLivesChanged;
        }

        private void OnLifeRestoring(long remainingTime)
        {
            if (remainingTimeInSeconds == remainingTime)
            {
                return;
            }
            
            remainingTimeInSeconds = remainingTime;
            infoText.text = DateTimeUtil.Format(remainingTimeInSeconds);
        }
        
        private void OnLivesChanged()
        {
            if (lifeCount == UserManager.PlayerData.lives)
            {
                return;
            }
            
            lifeCount = UserManager.PlayerData.lives;
            amountText.text = lifeCount.ToString();
            
            if (lifeCount == Global.GameSettingsConfig.maxLives)
            {
                infoText.text = "FULL";
            }
        }

    }
}
