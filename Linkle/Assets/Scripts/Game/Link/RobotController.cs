using System;
using System.Collections.Generic;
using Common.Timer;
using Game.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Link
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private LinkPlayerIndicator playIndicator;
        [SerializeField] private LinkComboAnimPlayer comboAnimPlayer;
        public event Action<int> OnGameFinished;

        private int robotIndex;
        private List<ComboDelayInfo> comboDelayInfos = new List<ComboDelayInfo>();
        private int currentIndex;
        private int remainingPairs;
        private TimerToken delayTimerToken;
        private int currentComboCount;
        
        public void Init(int robotIndex, LinkLevelInfo levelInfo, AgentData agentData)
        {
            this.robotIndex = robotIndex;
            remainingPairs = levelInfo.remainingPairs;
            playIndicator.Init(agentData.Avatar, remainingPairs);
            
            var robotData = levelInfo.robotData;
            var timings = new List<float>();
            for (var i = 0; i < remainingPairs; i++)
            {
                timings.Add(Random.Range(robotData.minDuration, robotData.maxDuration));
            }
            
            comboDelayInfos.Add(new ComboDelayInfo {delay = timings[0]});

            var comboLimitDuration = Global.GameSettingsConfig.comboJudgementInterval;
            for (var i = 1; i < remainingPairs; i++)
            {
                var comboDelayInfo = new ComboDelayInfo {isCombo = CheckCombo(i, timings[i], robotData)};
                if (comboDelayInfo.isCombo)
                {
                    if (comboLimitDuration <= robotData.minDuration)
                    {
                        comboDelayInfo.delay = comboLimitDuration;
                    }
                    else
                    {
                        comboDelayInfo.delay = Random.Range(robotData.minDuration, comboLimitDuration);
                    }
                }
                else
                {
                    var min = Mathf.Max(robotData.minDuration, comboLimitDuration);
                    var max = Mathf.Max(robotData.maxDuration, comboLimitDuration);
                    if (min > max)
                    {
                        (min, max) = (max, min);
                    }
                    comboDelayInfo.delay = Random.Range(min, max);
                }
                comboDelayInfos.Add(comboDelayInfo);
            }
        }
        
        public void Dispose()
        {
            CancelTimer();
            comboAnimPlayer.StopAnim();
        }
        
        public void BeginPlay()
        {
            currentIndex = 0;
            StartNextTimer();
        }
        
        public void EndPlay()
        {
            CancelTimer();
            comboAnimPlayer.StopAnim();
        }
        
        private bool CheckCombo(int index, float timing, RobotData robotData)
        {
            if (index < 1)
            {
                return false;
            }
            var testNumber = Random.Range(0f, 1f);
            return testNumber < 0.3f;
        }

        private void StartNextTimer()
        {
            delayTimerToken = TimerManager.After(comboDelayInfos[currentIndex].delay, Tick, true, Global.GamingTimerGroupId);
        }
        
        private bool CanCombo()
        {
            return comboDelayInfos[currentIndex].isCombo;
        }
        
        private void CancelTimer()
        {
            TimerManager.Cancel(delayTimerToken);
            delayTimerToken = null;
        }
        
        private void Tick()
        {
            remainingPairs--;

            OnTilesPaired();
            
            if (CanCombo())
            {
                currentComboCount++;
                if (currentComboCount >= 2)
                {
                    OnComboEnter();
                }
            }
            else if (currentComboCount > 1)
            {
                currentComboCount = 0;
                OnComboExit();
            }
            else if (currentComboCount > 0)
            {
                currentComboCount = 0;
            }
            
            if (remainingPairs <= 0)
            {
                GameOver();
            }
            else
            {
                currentIndex++;
                if (currentIndex < comboDelayInfos.Count)
                {
                    StartNextTimer();
                }
            }
        }

        private void OnTilesPaired()
        {
            playIndicator.SetRemainingPairs(remainingPairs);
        }
        
        private void OnComboEnter()
        {
            comboAnimPlayer.ShowCombo(currentComboCount);
        }
        
        private void OnComboExit()
        {
        }

        public void OnPause(bool paused)
        {
        }
        
        private void GameOver()
        {
            OnGameFinished?.Invoke(robotIndex);
        }
        
        private class ComboDelayInfo
        {
            public float delay;
            public bool isCombo;
        }
    }
}
