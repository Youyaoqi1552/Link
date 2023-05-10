using Common.UI;
using Common.Utility;
using Game.Window;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public enum MapNodeStatus
    {
        Active,
        Locked,
        Finished,
    }

    public class MapNode : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteAtlasSheet spriteSheet;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image[] stars;

        protected int targetLevel;
        protected MapNodeStatus status;

        public int TargetLevel => targetLevel;
        public MapNodeStatus Status => status;

        public virtual void Init(int level)
        {
            targetLevel = level;

            levelText.text = $"{targetLevel}";

            var winStars = 0;
            // var levelData = UserManager.GetLevelData(targetLevel);
            // if (levelData != null)
            // {
            //     winStars = levelData.winStars;
            // }

            for (var i = 0; i < 3; i++)
            {
                stars[i].enabled = i < winStars;
            }
        }

        public virtual void Dispose()
        {
        }

        public void SetStatus(MapNodeStatus nodeStatus)
        {
            status = nodeStatus;
            switch (status)
            {
                case MapNodeStatus.Active:
                    levelText.enabled = true;
                    icon.sprite = spriteSheet.GetSprite("map_node_unlocked");
                    break;

                case MapNodeStatus.Finished:
                    levelText.enabled = true;
                    icon.sprite = spriteSheet.GetSprite("map_node_finished");
                    break;

                case MapNodeStatus.Locked:
                    levelText.enabled = false;
                    icon.sprite = spriteSheet.GetSprite("map_node_locked");
                    break;
            }
            icon.SetNativeSize();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (MapNodeStatus.Locked == status)
            {
                return;
            }

            var context = new DataBundle();
            context.Set("level", targetLevel);
            WindowManager.Shared.PushWindow<GameMatchingWindow>(context);
        }
    }
}
