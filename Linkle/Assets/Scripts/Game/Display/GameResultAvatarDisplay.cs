using Game.Data;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Display
{
    public class GameResultAvatarDisplay : MonoBehaviour
    {
        [SerializeField] private UIAvatar avatar;
        [SerializeField] private Image rankIcon;
        
        public void SetData(int rank, AgentData agentData)
        {
            avatar.SetAvatar(agentData.Avatar);
        }
    }
}
