using System.Collections.Generic;
using Common.Timer;
using Common.UI;
using Common.Utility;
using Game.Data;
using Game.Display;
using Game.Manager;
using Game.Scene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Window
{
    public class GameMatchingWindow : UIWindow
    {
        [SerializeField] private Text levelText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameMatchingAvatarDisplay[] avatarDisplays;
        [SerializeField] private Image actionBg;
        [SerializeField] private Image actionLabel;
        [SerializeField] private SpriteAtlasSheet spriteSheet;
        
        private ActionState actionState;
        private int currentLevel;
        private List<AgentData> allAgentData = new List<AgentData>();
        private int currentAgentIndex;
        
        private int searchingAnimIndex;
        private TimerToken searchingTimer;
        private TimerToken delayTimerToken;
        private int enterSceneCounter;
        
        private SceneLoadCancellationToken sceneLoadCancellationToken;
        private SceneLoadCancellationCallback sceneLoadCancellationCallback;
        
        public override void OnCreate(DataBundle context)
        {
            base.OnCreate(context);
            
            actionState = ActionState.Play;
            enterSceneCounter = 2;
            currentAgentIndex = 1;
            currentLevel = context.Get<int>("level");
            
            allAgentData.Clear();
            for (var i = 0; i < avatarDisplays.Length; i++)
            {
                if (0 == i)
                {
                    var playerData = UserManager.PlayerData;
                    var agentData = new AgentData
                    {
                        IsSelf = true,
                        Name = playerData.name,
                        Avatar = playerData.avatar,
                    };
                    allAgentData.Add(agentData);
                    avatarDisplays[i].SetData(agentData);
                }
                else
                {
                    var agentData = new AgentData
                    {
                        Name = $"Player{allAgentData.Count + 1}",
                        Avatar = new AvatarData {Id = 100004},
                    };
                    allAgentData.Add(agentData);
                    avatarDisplays[i].SetEmpty();
                }
            }
                
            levelText.text = $"{currentLevel}";
            messageText.text = "Search the opponent.";
            
            actionBg.sprite = spriteSheet.GetSprite("btn_play");
            actionBg.SetNativeSize();
            actionLabel.sprite = spriteSheet.GetSprite("label_play");
            actionLabel.SetNativeSize();
            
            PreloadGameScene();
        }
        
        public override void OnDispose()
        {
            sceneLoadCancellationToken = null;
            sceneLoadCancellationCallback = null;
            base.OnDispose();
        }
        
        private void PreloadGameScene()
        {
            var bundle = new DataBundle();
            bundle.Set("level", currentLevel);
            bundle.Set("agents", allAgentData.ToArray());
            sceneLoadCancellationToken = SceneNavigator.Shared.GotoGame(bundle);
            sceneLoadCancellationToken.completed += OnSceneLoadCancelCallback;
        }
        
        private void OnSceneLoadCancelCallback(SceneLoadCancellationCallback cancellationCallback)
        {
            sceneLoadCancellationCallback = cancellationCallback;
            if (sceneLoadCancellationCallback.Cancelled)
            {
                CloseSelf();
            }
            else
            {
                EnterGameScene();
            }
        }
        
        private void EnterGameScene()
        {
            enterSceneCounter--;
            if (enterSceneCounter > 0)
            {
                return;
            }
            
            CloseSelf();
            sceneLoadCancellationCallback.Callback?.Invoke(); 
        }
        
        private void MatchPlayer()
        {
            avatarDisplays[currentAgentIndex].SetData(allAgentData[currentAgentIndex]);

            currentAgentIndex++;
            if (currentAgentIndex < avatarDisplays.Length)
            {
                MatchNextPlayer();
            }
            else
            {
                actionState = ActionState.Closing;
                delayTimerToken = TimerManager.After(0.5f, EnterGameScene);
            }
        }
        
        private void MatchNextPlayer()
        {
            var delay = Random.Range(0.2f, 2f);
            delayTimerToken = TimerManager.After(delay, MatchPlayer);
        }
        
        public void OnActionPressed()
        {
            switch (actionState)
            {
                case ActionState.Play:
                    actionState = ActionState.Matching;
                    
                    actionBg.sprite = spriteSheet.GetSprite("btn_cancel");
                    actionBg.SetNativeSize();

                    actionLabel.sprite = spriteSheet.GetSprite("label_cancel");
                    actionLabel.SetNativeSize();
                    
                    searchingAnimIndex = 3;
                    OnSearching(0);
                    
                    searchingTimer = TimerManager.Every(0.5f, OnSearching);

                    MatchNextPlayer();
                    break;
                
                case ActionState.Matching:
                    OnClosePressed();
                    break;
                
                case ActionState.Closing:
                    break;
            }
        }
        
        public void OnClosePressed()
        {
            if (ActionState.Closing == actionState)
            {
                return;
            }
            
            actionState = ActionState.Closing;
            CloseSelf();
            
            SceneNavigator.Shared.CancelToken(sceneLoadCancellationToken);
            sceneLoadCancellationToken = null;
        }
        
        private void OnSearching(float _)
        {
            switch (searchingAnimIndex)
            {
                case 0:
                    messageText.text = "Searching";
                    break;
                case 1:
                    messageText.text = "Searching.";
                    break;
                
                case 2:
                    messageText.text = "Searching..";
                    break;
                
                case 3:
                    messageText.text = "Searching...";
                    break;
            }
            
            searchingAnimIndex++;
            if (searchingAnimIndex > 3)
            {
                searchingAnimIndex = 0;
            }
        }
        
        public virtual void RemoveTimers()
        {
            TimerManager.Cancel(delayTimerToken);
            delayTimerToken = null;
            
            TimerManager.Cancel(searchingTimer);
            searchingTimer = null;
        }
        
        public override void OnCloseWillStart()
        {
            RemoveTimers();
            base.OnCloseWillStart();
        }

        public override void OnCloseImmediately()
        {
            RemoveTimers();
            base.OnCloseImmediately();
        }
        
        private enum ActionState
        {
            Play,
            Matching,
            Closing,
        }
    }
}