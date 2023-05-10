using System.Collections.Generic;
using Common.Utility;
using Game.Data;
using Game.Define;
using Game.Event;

namespace Game.Manager
{
    public class UserManager
    {
        private const string PlayerSaveFileName = "playerSaveData.dat";
        private const string LifeRestoringTimerLabel = "lifeRestoring";
        
        public static PlayerData PlayerData;
        private static long lifeRestoringSeconds;

        private static Dictionary<int, int> items = new Dictionary<int, int>();

        public static void LoadPlayerData()
        {
            PlayerData = SaveSystem.LoadPlayerData(PlayerSaveFileName);
            if (null == PlayerData)
            {
                PlayerData = new PlayerData
                {
                    name = "Guest",
                    level = 1,
                    avatar = new AvatarData(),
                    credits = Global.GameSettingsConfig.defaultCredits,
                    lives = Global.GameSettingsConfig.maxLives,
                    lifeRestoreTime = -1
                };

                PlayerData.items = new List<ItemSaveData>
                {
                    new ItemSaveData {itemType = (int) ItemType.Hint, amount = 5},
                    new ItemSaveData {itemType = (int) ItemType.Shuffle, amount = 5},
                    new ItemSaveData {itemType = (int) ItemType.GameTheme, amount = 5}
                };
            }
            else if (PlayerData.lives < Global.GameSettingsConfig.maxLives)
            {
                if (PlayerData.lifeRestoreTime < 0)
                {
                    PlayerData.lives = Global.GameSettingsConfig.maxLives;
                    PlayerData.lifeRestoreTime = -1;
                }
                else
                {
                    var nowTime = DateTimeUtil.GetEpochTimeSeconds();
                    var restoredCount = (nowTime - PlayerData.lifeRestoreTime) / Global.GameSettingsConfig.lifeRestoreDuration;
                    PlayerData.lives += (int)restoredCount;
                    if (PlayerData.lives >= Global.GameSettingsConfig.maxLives)
                    {
                        PlayerData.lives = Global.GameSettingsConfig.maxLives;
                        PlayerData.lifeRestoreTime = -1;
                    }
                    else
                    {
                        PlayerData.lifeRestoreTime += restoredCount * Global.GameSettingsConfig.lifeRestoreDuration;
                        lifeRestoringSeconds = nowTime - PlayerData.lifeRestoreTime;
                        WorldTimeManager.Add(LifeRestoringTimerLabel, OnLifeRestoring);
                    }
                }
            }
            else
            {
                PlayerData.lives = Global.GameSettingsConfig.maxLives;
                PlayerData.lifeRestoreTime = -1;
            }
            
            for (var i = 0; i < PlayerData.items.Count; i++)
            {
                var item = PlayerData.items[i];
                items[item.itemType] = i;
            }
        }

        public static void SavePlayerData()
        {
            SaveSystem.SavePlayerData(PlayerData, PlayerSaveFileName);
        }

        public static void CompleteLevel(LevelCompleteData levelCompleteData)
        {
            if (PlayerData.level == levelCompleteData.Level)
            {
                if (levelCompleteData.IsWin)
                {
                    PlayerData.level = levelCompleteData.Level + 1;
                    PlayerData.winningStreaks++;
                }
                else
                {
                    PlayerData.winningStreaks--;
                }
            }
            AddCredits(levelCompleteData.RewardedCredits);
            SavePlayerData();
            
            LevelManager.CompleteLevel(levelCompleteData);
        }
        
        public static void AddCredits(int value)
        {
            var oldCredits = PlayerData.credits;
            PlayerData.credits -= value;
            if (PlayerData.credits < 0)
            {
                PlayerData.credits = 0;
            }
            if (oldCredits != PlayerData.credits)
            {
                PlayerEvent.InvokeCreditsChangedEvent();
            }
        }

        public static long GetLifeRestoringRemainingTime()
        {
            if (PlayerData.lives == Global.GameSettingsConfig.maxLives)
            {
                return 0;
            }
            return Global.GameSettingsConfig.lifeRestoreDuration - lifeRestoringSeconds;
        }

        public static void AddLives(int lives)
        {
            if (lives <= 0)
            {
                return;
            }

            var oldLifeCount = PlayerData.lives;
            PlayerData.lives += lives;
            if (PlayerData.lives >= Global.GameSettingsConfig.maxLives)
            {
                WorldTimeManager.Remove(LifeRestoringTimerLabel);
                lifeRestoringSeconds = 0;
                PlayerData.lifeRestoreTime = -1;
                PlayerData.lives = Global.GameSettingsConfig.maxLives;
            }
            SavePlayerData();

            if (oldLifeCount != PlayerData.lives)
            {
                PlayerEvent.InvokeLivesChangedEvent();
            }
        }
        
        public static void ConsumeOneLife()
        {
            if (PlayerData.lives > 1)
            {
                var oldLifeCount = PlayerData.lives;
                PlayerData.lives--;
                if (oldLifeCount == Global.GameSettingsConfig.maxLives)
                {
                    lifeRestoringSeconds = 0;
                    PlayerData.lifeRestoreTime = DateTimeUtil.GetEpochTimeSeconds();
                    WorldTimeManager.Add(LifeRestoringTimerLabel, OnLifeRestoring);
                    
                    PlayerEvent.InvokeLifeRestoringEvent(Global.GameSettingsConfig.lifeRestoreDuration);
                }
                SavePlayerData();
                PlayerEvent.InvokeLivesChangedEvent();
            }
        }

        private static void OnLifeRestoring(long seconds)
        {
            lifeRestoringSeconds += seconds;
            
            var restoredCount = (int)(lifeRestoringSeconds / Global.GameSettingsConfig.lifeRestoreDuration);
            if (restoredCount > 0)
            {
                var oldLifeCount = PlayerData.lives;
                PlayerData.lives += restoredCount;
                if (PlayerData.lives >= Global.GameSettingsConfig.maxLives)
                {
                    WorldTimeManager.Remove(LifeRestoringTimerLabel);
                    lifeRestoringSeconds = 0;
                    PlayerData.lifeRestoreTime = -1;
                    PlayerData.lives = Global.GameSettingsConfig.maxLives;
                }
                else
                {
                    var duration = restoredCount * Global.GameSettingsConfig.lifeRestoreDuration;
                    PlayerData.lifeRestoreTime += duration;
                    lifeRestoringSeconds -= duration;
                    PlayerEvent.InvokeLifeRestoringEvent(Global.GameSettingsConfig.lifeRestoreDuration - lifeRestoringSeconds);
                }
                
                if (oldLifeCount != PlayerData.lives)
                {
                    PlayerEvent.InvokeLivesChangedEvent();
                }
            }
            else
            {
                PlayerEvent.InvokeLifeRestoringEvent(Global.GameSettingsConfig.lifeRestoreDuration - lifeRestoringSeconds);
            }
        }

        public static int GetItemAmount(ItemType itemType)
        {
            return PlayerData.items[items[(int) itemType]].amount;
        }

        public static bool TryUesItem(ItemType itemType)
        {
            var idx = items[(int) itemType];
            var itemData = PlayerData.items[idx];
            if (itemData.amount > 0)
            {
                itemData.amount--;
                SavePlayerData();
                return true;
            }
            return false;
        }
        
        public static bool TryBuyItem(ItemType itemType, int price)
        {
            if (PlayerData.credits >= price)
            {
                PlayerData.credits -= price;
                SavePlayerData();
                PlayerEvent.InvokeCreditsChangedEvent();
                return true;
            }
            return false;
        }
    }
}
