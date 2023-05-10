using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MapNodeIndicator : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private UIAvatar avatarDisplay;

        public void SetAvatar(AvatarData avatarData)
        {
            // avatarDisplay.SetAvatar(avatarData);
        }
    }
}