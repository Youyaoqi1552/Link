using Common.Timer;
using Common.UI;
using Common.Utility;
using Game.Anim;
using Game.Data;
using Game.Define;
using Game.Event;
using Game.Link;
using Game.Manager;
using Game.Scene;
using Game.Window;
using UnityEngine;

namespace Game.Controller
{
    public class GameController : SceneController
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private RobotController[] robotControllers;
        [SerializeField] private LinkItemEntry[] itemEntries;
        [SerializeField] private LinkItemBuyHintDisplay itemBuyHintDisplay;
        [SerializeField] private GameStartAnimator gameStartAnimator;
        
        private int currentLevel;
        private AgentData[] currentAgents;
        private int selfRank;
        private LinkLevelInfo currentLevelInfo;

        public override void OnCreate(DataBundle context)
        {
            base.OnCreate(context);
            
            currentLevel = context.Get<int>("level");
            currentAgents = context.Get<AgentData[]>("agents");
            
            GlobalEvent.GamePause += OnGamePause;
        }
        
        public override void OnDispose()
        {
            GlobalEvent.GamePause -= OnGamePause;

            playerController.Dispose();
            foreach (var robotController in robotControllers)
            {
                robotController.Dispose();
            }

            itemBuyHintDisplay.Dispose();
            
            LinkAssetsManager.Current.Dispose();
            base.OnDispose();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            UserManager.ConsumeOneLife();
            
            currentLevelInfo = LevelManager.PlayLevel(currentLevel);
            
            LinkAssetsManager.Current.onAllAssetsLoaded += OnAllAssetsLoaded;
            LinkAssetsManager.Current.LoadAssets(currentLevelInfo);
        }

        private void OnAllAssetsLoaded()
        {
            selfRank = 0;
            
            foreach (var itemEntry in itemEntries)
            {
                itemEntry.Init();
                itemEntry.OnPressed += TryUseItem;
            }

            playerController.Init(currentLevelInfo, currentAgents[0]);
            playerController.OnGameFinished += OnPlayerFinished;
            
            for (var i = 0; i < robotControllers.Length; i++)
            {
                var robotController = robotControllers[i];
                robotController.Init(i, currentLevelInfo.Clone(), currentAgents[i + 1]);
                robotController.OnGameFinished += OnRobotFinished;
            }
            
            gameStartAnimator.PlayAnim(currentLevel, currentAgents);
        }

        public void OnGameStart()
        {
            playerController.BeginPlay();

            foreach (var robotController in robotControllers)
            {
                robotController.BeginPlay();
            }
        }
        
        private void OnPlayerFinished()
        {
            selfRank = 1;
            OnGameFinish();
        }

        private void OnRobotFinished(int robotIndex)
        {
            selfRank = 2;
            OnGameFinish();
        }
        
        private void OnGameFinish()
        {
            playerController.EndPlay();
            foreach (var robotController in robotControllers)
            {
                robotController.EndPlay();
            }

            var levelCompleteData = new LevelCompleteData
            {
                Level = currentLevel,
                Ranking = selfRank,
                Agents = currentAgents,
            };
            if (levelCompleteData.IsWin)
            {
                levelCompleteData.WinStars = 3;
                levelCompleteData.MaxComboCount = playerController.maxComboCount;
                levelCompleteData.RewardedMedals = playerController.totalRewardedMedals;
                levelCompleteData.RewardedCredits = levelCompleteData.WinStars * 10;
            }

            UserManager.CompleteLevel(levelCompleteData);
            
            var context = new DataBundle();
            context.Set("levelCompleteData", levelCompleteData);
            WindowManager.Shared.PushWindow<GameResultWindow>(context);
        }
        
        private void OnGamePause(bool paused)
        {
            playerController.OnPause(paused);

            foreach (var robotController in robotControllers)
            {
                robotController.OnPause(paused);
            }
            
            if (paused)
            {
                TimerManager.Pause(Global.GamingTimerGroupId);
            }
            else
            {
                TimerManager.Resume(Global.GamingTimerGroupId);
            }
        }

        public void OnSettingsPressed()
        {
            GlobalEvent.InvokeGamePauseEvent(true);
            WindowManager.Shared.PushWindow<SettingsWindow>();
        }

        public void OnChatPressed()
        {
        }
        
        private void TryUseItem(LinkItemEntry itemEntry)
        {
            if (selfRank > 0 || !playerController.CanUseItem())
            {
                return;
            }

            var itemType = itemEntry.ItemType;
            if (UserManager.TryUesItem(itemType))
            {
                itemEntry.Refresh();
                UseItem(itemType);
            }
            else
            {
                var price = Global.GetItemPrice(itemType);
                if (UserManager.TryBuyItem(itemType, price))
                {
                    itemBuyHintDisplay.Show(price);
                    
                    itemEntry.Refresh();
                    UseItem(itemType);
                }
                else
                {
                    Toast.MakeToast("Not enough coins.");
                }
            }
        }

        private void UseItem(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Hint:
                    playerController.Hint();
                    break;
                    
                case ItemType.Shuffle:
                    playerController.Shuffle();
                    break;
                    
                case ItemType.GameTheme:
                    playerController.ChangeTheme();
                    break;
            }
        }
    }
}
