using System;
using UnityEngine;

namespace Game.Config
{

    [Serializable]
    public class TileConfig
    {
        public int tag;
    }
    
    [Serializable]
    public class FallingConfig
    {
        public int direction;
        public int step;
    }
    
    [Serializable]
    public class BoardConfig
    {
        public int width;
        public int height;
        public int maxPairs;
        public FallingConfig[] fallingConfigs;
        public TileConfig[] tileConfigs;
        public int difficulty = -1;
    }

    [Serializable]
    public class LevelConfig
    {
        public BoardConfig[] boardConfigs;
    }
    
    public class LevelDatabase : ScriptableObject
    {
        public LevelConfig[] levels;
    }
}
