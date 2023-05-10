using Common.UI;
using Common.Utility;
using Game.Data;
using Game.Display;
using Game.Scene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public class GameResultWindow : UIWindow
    {
        [Header("Common")]
        [SerializeField] private GameResultAvatarDisplay[] avatarDisplays;
        [SerializeField] private TextMeshProUGUI comboPairsRewardedMedalText;
        [SerializeField] private TextMeshProUGUI starRewardedMedalText;
        [SerializeField] private TextMeshProUGUI pairsRewardedMedalText;
        
        [Header("Win Group")]
        [SerializeField] private RectTransform winGroup;
        [SerializeField] private Image[] winStars;
        [SerializeField] private Image winLight;

        [Header("Fail Group")]
        [SerializeField] private RectTransform failGroup;
        [SerializeField] private Image[] failStars;
        [SerializeField] private Image failLight;

        private LevelCompleteData levelCompleteData;
        
        public override void OnCreate(DataBundle context)
        {
            base.OnCreate(context);

            levelCompleteData = context.Get<LevelCompleteData>("levelCompleteData");

            for (var i = 0; i < avatarDisplays.Length; i++)
            {
                avatarDisplays[i].SetData(i+1, levelCompleteData.Agents[i]);
            }
            
            if (levelCompleteData.IsWin)
            {
                failGroup.gameObject.SetActive(false);
                failLight.enabled = false;

                for (var i = 0; i < winStars.Length; i++)
                {
                    winStars[i].enabled = i <= levelCompleteData.WinStars;
                }
                winGroup.gameObject.SetActive(true);
                winLight.enabled = true;

                comboPairsRewardedMedalText.text = $"+{levelCompleteData.MaxComboCount * 5}";
                starRewardedMedalText.text = $"+{levelCompleteData.WinStars * 3}";
                pairsRewardedMedalText.text = $"+{levelCompleteData.RewardedMedals}";
            }
            else
            {
                winGroup.gameObject.SetActive(false);
                winLight.enabled = false;
                
                for (var i = 0; i < winStars.Length; i++)
                {
                    failStars[i].enabled = i <= levelCompleteData.WinStars;
                }
                failGroup.gameObject.SetActive(true);
                failLight.enabled = true;
            }
        }
        
        public void OnContinuePressed()
        {
            CloseSelf(true);
            SceneNavigator.Shared.GotoHome();
        }
    }
}
