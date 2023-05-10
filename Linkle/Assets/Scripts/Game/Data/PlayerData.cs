using System.Collections.Generic;
using ProtoBuf;

namespace Game.Data
{
    [ProtoContract]
    public class PlayerData
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public int level;

        [ProtoMember(3)]
        public int credits;

        [ProtoMember(4)]
        public int lives;
        
        [ProtoMember(5)]
        public long lifeRestoreTime;

        [ProtoMember(6)]
        public AvatarData avatar = new AvatarData();

        [ProtoMember(7)]
        public List<ItemSaveData> items;
        
        [ProtoMember(8)]
        public int winningStreaks;
    }
    
    [ProtoContract]
    public class ItemSaveData
    {
        [ProtoMember(1)]
        public int itemType;

        [ProtoMember(2)]
        public int amount;
    }
}