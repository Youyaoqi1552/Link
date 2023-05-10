using System;

namespace Game.Config
{
    [Serializable]
    public class PatternDifficultyConfig
    {
        public int id;
        public string theme;
        public int difficultyLevel;
        public int[] tiles;
    }
}