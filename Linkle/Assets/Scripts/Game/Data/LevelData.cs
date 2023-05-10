using System.Collections.Generic;
using ProtoBuf;

namespace Game.Data
{
    public class LevelCompleteData
    {
        public int Level;
        public int Ranking;
        public int WinStars;
        public int MaxComboCount;
        public int RewardedMedals;
        public int RewardedCredits;
        public AgentData[] Agents;

        public bool IsWin => Ranking == 1;
    }

    [ProtoContract]
    public class LevelSaveData
    {
        [ProtoMember(1)]
        public int level;
        
        [ProtoMember(2)]
        public int tryCount;
        
        [ProtoMember(3)]
        public int ranking;

        [ProtoMember(4)]
        public int winStars;

        [ProtoMember(5)]
        public int maxComboCount;
    }

    [ProtoContract]
    public class LevelSaveDatabase
    {
        [ProtoMember(1)]
        public List<LevelSaveData> levels;
    }
}
