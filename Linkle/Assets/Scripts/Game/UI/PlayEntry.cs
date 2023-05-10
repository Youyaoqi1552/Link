using Common.UI;
using Common.Utility;
using Game.Window;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayEntry : UITabButton
    {
        [SerializeField] private TMP_Text levelText;

        private int currentLevel;
        
        public void Init(int level)
        {
            currentLevel = level;
            levelText.text = $"{currentLevel}";
        }
        
        public override void Select()
        {
        }

        public override void Deselect()
        {
        }
        
        public override void RepeatSelect()
        {
            base.RepeatSelect();

            var context = new DataBundle();
            context.Set("level", currentLevel);
            WindowManager.Shared.PushWindow<GameMatchingWindow>(context);
        }
    }
}
