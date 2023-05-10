using Game.Data;
using Game.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIAvatar : MonoBehaviour
    {
        [SerializeField] protected RawImage icon;

        public void SetEmpty()
        {
            icon.enabled = false;
        }
        
        public void SetAvatar(AvatarData avatarData)
        {
            icon.enabled = true;
            icon.texture = AvatarManager.GetLocalAvatarById(avatarData.Id);
        }
    }
}
