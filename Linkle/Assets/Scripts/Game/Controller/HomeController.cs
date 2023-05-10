using System.Collections.Generic;
using Common.UI;
using Game.Data;
using Game.Display;
using Game.Event;
using Game.Manager;
using Game.Scene;
using Game.UI;
using Game.Window;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controller
{
    public class HomeController : SceneController
    {
        [SerializeField] private UIAvatar playerAvatar;
        [SerializeField] private LifeDisplay lifeDisplay;
        [SerializeField] private CreditDisplay creditDisplay;
        [SerializeField] private PlayEntry playEntry;
        [SerializeField] private ScrollRect mapScrollRect;
        [SerializeField] private RectTransform mapNodeContainer;
        [SerializeField] private MapNodeIndicator mapNodeIndicator;

        private List<MapNode> mapNodes = new List<MapNode>();
        
        private void Start()
        {
            var playerData = UserManager.PlayerData;
            playerAvatar.SetAvatar(playerData.avatar);
            
            lifeDisplay.Init();
            creditDisplay.SetAmount(playerData.credits);

            PlayerEvent.CreditsChanged += OnCreditsChanged;

            playEntry.Init(playerData.level);
            playEntry.SetSelect();

            var maxLevel = LevelManager.MaxLevel;
            var maxUnlockedLevel = playerData.level;
            for (var i = 0; i < maxLevel; i++)
            {
                var level = i + 1;
                var mapNode = mapNodeContainer.GetChild(i).GetComponent<MapNode>();
                mapNode.Init(level);
                mapNodes.Add(mapNode);

                if (level < maxUnlockedLevel)
                {
                    mapNode.SetStatus(MapNodeStatus.Finished);
                }
                else if (level == maxUnlockedLevel)
                {
                    mapNode.SetStatus(MapNodeStatus.Active);
                    SetMapNodeIndicatorTarget(mapNode);
                }
                else
                {
                    mapNode.SetStatus(MapNodeStatus.Locked);
                }
            }
            mapNodeIndicator.SetAvatar(new AvatarData());
            
            var currentLevel = maxUnlockedLevel;
            var mapNodeAnchoredPos = mapNodes[currentLevel - 1].GetComponent<RectTransform>().anchoredPosition;
            var viewportSize = mapScrollRect.viewport.rect.size;
            var halfViewportHeight = viewportSize.y * 0.5f;
            var contentSize = mapScrollRect.content.rect.size;
            mapScrollRect.verticalNormalizedPosition = Mathf.Clamp01((mapNodeAnchoredPos.y - halfViewportHeight) / (contentSize.y - viewportSize.y));
        }
        
        public override void OnDispose()
        {
            PlayerEvent.CreditsChanged -= OnCreditsChanged;
            
            lifeDisplay.Dispose();
            base.OnDispose();
        }

        private void OnCreditsChanged()
        {
            creditDisplay.SetAmount(UserManager.PlayerData.credits);
        }
        
        private void SetMapNodeIndicatorTarget(MapNode mapNode)
        {
            var nodeRect = mapNode.GetComponent<RectTransform>();
            var indicatorRect = mapNodeIndicator.GetComponent<RectTransform>();
            indicatorRect.anchoredPosition = nodeRect.anchoredPosition;
        }

        public void OnUserAvatarPressed()
        {
            WindowManager.Shared.PushWindow<UserProfileWindow>();
        }
        
        public void OnRankEntryPressed()
        {
            WindowManager.Shared.PushWindow<RankingListWindow>();
        }
        
        public void OnChestEntryPressed()
        {
            // WindowManager.Shared.PushWindow<ChestWindow>();
        }
        
        public void OnPiggyBankPressed()
        {
            WindowManager.Shared.PushWindow<PiggyBankWindow>();
        }
    }
}
