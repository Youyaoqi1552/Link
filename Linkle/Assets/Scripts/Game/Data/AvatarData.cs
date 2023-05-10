using ProtoBuf;

namespace Game.Data
{
    [ProtoContract]
    public class AvatarData
    {
        [ProtoMember(1)]
        public int Id = 100001;
        
        [ProtoMember(2)]
        public string Url;
    }
}