using System;

namespace Game.Config
{
    [Serializable]
    public class RobotDifficultyConfig
    {
        public int difficultyLevel;
        public int minBoardDifficultyLevel;
        public int maxBoardDifficultyLevel;
        public float minReactTime;
        public float maxReactTime;
    }
}