using UnityEngine;

namespace Game.Config
{
    public class GameSettingsConfig : ScriptableObject
    {
        public int defaultCredits = 500;
        
        public int maxLives = 5;
        public long lifeRestoreDuration;
        public int refillLifeCostCredits;
        
        public int basicTileRewardScore;
        public float doubleScoreDuration;
        public float comboJudgementInterval;

        public int recallLevels;
        public int baseStatusValue;
        public int winTimes;
        public int[] rankBonusScores;
    }
}
