using Game.Data;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Display
{
    public class GameMatchingAvatarDisplay : MonoBehaviour
    {
        [SerializeField] private UIAvatar avatar;
        [SerializeField] private Image placeholder;
        
        public void SetEmpty()
        {
            if (null != placeholder)
            {
                placeholder.enabled = true;
            }
            avatar.SetEmpty();
        }
        
        public void SetData(AgentData agentData)
        {
            if (null != placeholder)
            {
                placeholder.enabled = false;
            }
            avatar.SetAvatar(agentData.Avatar);
        }
    }
}
