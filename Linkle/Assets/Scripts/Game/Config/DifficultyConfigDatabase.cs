using System;
using UnityEngine;

namespace Game.Config
{
    [Serializable]
    public class DifficultyJudgeConfig
    {
        public int minPlayerStateValue;
        public int maxPlayerStateValue;
        public int levelDifficulty;
        public int minRobotDifficultyLevel;
        public int maxRobotDifficultyLevel;
        public int patternDifficultyLevel;
    }

    public class DifficultyConfigDatabase : ScriptableObject
    {
        public DifficultyJudgeConfig[] difficultyJudgeConfigs;
        public PatternDifficultyConfig[] patternDifficultyConfigs;
        public RobotDifficultyConfig[] robotDifficultyConfigs;
    }
}