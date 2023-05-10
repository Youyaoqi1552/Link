using Game.Data;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Anim
{
    public class GameStartAnimator : MonoBehaviour
    {
        [Header("Game Start")]
        [SerializeField] private Animation gameStartAnimation;
        [SerializeField] private Text levelText;
        [SerializeField] private UIAvatar[] agentAvatars;
        [SerializeField] private TextMeshProUGUI[] agentNames;

        public void PlayAnim(int level, AgentData[] agents)
        {
            levelText.text = $"{level}";
            for (var i = 0; i < agents.Length; i++)
            {
                var agent = agents[i];
                agentNames[i].text = $"{agent.Name}";
                agentAvatars[i].SetAvatar(agent.Avatar);
            }

            gameStartAnimation.Play("GameStartAnim");
        }
    }
}
