using System;
using System.Collections.Generic;
using Common.Extensions;
using Game.Config;
using Game.Data;
using Game.Link;
using UnityEngine;

namespace Game.Manager
{
    public static class LevelManager
    {
        private const string LevelSaveFileName = "levelDatabase.dat";
        private static LevelSaveDatabase levelSaveDatabase;
        
        private static LevelDatabase levelDatabase;
        private static DifficultyConfigDatabase difficultyConfigDatabase;

        public static int MaxLevel => 50;
        
        public static void Init(LevelDatabase levels, DifficultyConfigDatabase config)
        {
            levelDatabase = levels;
            difficultyConfigDatabase = config;
            
            if (null == levelSaveDatabase)
            {
                levelSaveDatabase = SaveSystem.LoadLevelDatabase(LevelSaveFileName) ?? new LevelSaveDatabase
                {
                    levels = new List<LevelSaveData>()
                };
            }
        }
        
        public static LinkLevelInfo PlayLevel(int level)
        {
            var playerData = UserManager.PlayerData;
            var stateValue = 0;
            if (level >= playerData.level && level > Global.GameSettingsConfig.recallLevels)
            {
                for (var i = level - Global.GameSettingsConfig.recallLevels; i < level; i++)
                {
                    var levelData = levelSaveDatabase.levels[i];
                    stateValue += Global.GameSettingsConfig.rankBonusScores[levelData.ranking];
                }
                
                if (playerData.winningStreaks >= 2)
                {
                    stateValue += Global.GameSettingsConfig.winTimes * playerData.winningStreaks;
                }
            }
            else
            {
                stateValue = Global.GameSettingsConfig.baseStatusValue;
            }


            foreach (var difficultyJudgeConfig in difficultyConfigDatabase.difficultyJudgeConfigs)
            {
                if (difficultyJudgeConfig.minPlayerStateValue <= stateValue &&
                    stateValue <= difficultyJudgeConfig.maxPlayerStateValue)
                {
                    var levelConfig = levelDatabase.levels[level - 1];
                    var minDistance = int.MaxValue;
                    var unlimitedBoardConfigs = new List<BoardConfig>();
                    var availableBoardConfigs = new List<BoardConfig>();
                    foreach (var boardConfig in levelConfig.boardConfigs)
                    {
                        if (boardConfig.difficulty < 0)
                        {
                            unlimitedBoardConfigs.Add(boardConfig);
                        }
                        else
                        {
                            var distance = Math.Abs(boardConfig.difficulty - difficultyJudgeConfig.levelDifficulty);
                            if (distance == minDistance)
                            {
                                availableBoardConfigs.Add(boardConfig);
                            }
                            else if (distance < minDistance)
                            {
                                minDistance = distance;
                                availableBoardConfigs.Clear();
                                availableBoardConfigs.Add(boardConfig);
                            }
                        }
                    }

                    if (unlimitedBoardConfigs.Count > 0)
                    {
                        availableBoardConfigs.AddRange(unlimitedBoardConfigs);
                        unlimitedBoardConfigs.Clear();
                    }

                    var availableRobotDifficultyConfigs = new List<RobotDifficultyConfig>();
                    foreach (var robotDifficultyConfig in  difficultyConfigDatabase.robotDifficultyConfigs)
                    {
                        if (difficultyJudgeConfig.minRobotDifficultyLevel <= robotDifficultyConfig.difficultyLevel &&
                            robotDifficultyConfig.difficultyLevel <= difficultyJudgeConfig.maxRobotDifficultyLevel)
                        {
                            if (robotDifficultyConfig.minBoardDifficultyLevel <= difficultyJudgeConfig.levelDifficulty &&
                                difficultyJudgeConfig.levelDifficulty <= robotDifficultyConfig.maxBoardDifficultyLevel)
                            {
                                availableRobotDifficultyConfigs.Add(robotDifficultyConfig);
                            }
                        }
                    }

                    var availablePatternConfigs = new List<PatternDifficultyConfig>();
                    foreach (var patternDifficultyConfig in difficultyConfigDatabase.patternDifficultyConfigs)
                    {
                        if (patternDifficultyConfig.difficultyLevel == difficultyJudgeConfig.patternDifficultyLevel)
                        {
                            availablePatternConfigs.Add(patternDifficultyConfig);
                        }
                    }
                    
                    return LinkLevelInfo.Create(level, availableBoardConfigs[availableBoardConfigs.Choice()], availableRobotDifficultyConfigs[availableRobotDifficultyConfigs.Choice()], availablePatternConfigs);
                }
            }
            Debug.LogError($"==========>{level}");
            return null;
        }
        
        public static void CompleteLevel(LevelCompleteData levelCompleteData)
        {
            if (levelSaveDatabase.levels.Count < levelCompleteData.Level)
            {
                var diff = levelCompleteData.Level - levelSaveDatabase.levels.Count;
                for (var i = 0; i < diff; i++)
                {
                    levelSaveDatabase.levels.Add(null);
                }
            }

            var idx = levelCompleteData.Level - 1;
            var levelSaveData = levelSaveDatabase.levels[idx];
            if (null == levelSaveData)
            {
                levelSaveData = new LevelSaveData {level = levelCompleteData.Level}; 
                levelSaveDatabase.levels[idx] = levelSaveData;
            }
            levelSaveData.tryCount++;
            if (levelSaveData.ranking > levelCompleteData.Ranking)
            {
                levelSaveData.ranking = levelCompleteData.Ranking;
            }
            if (levelSaveData.winStars < levelCompleteData.WinStars)
            {
                levelSaveData.winStars = levelCompleteData.WinStars;
            }
            if (levelSaveData.maxComboCount < levelCompleteData.MaxComboCount)
            {
                levelSaveData.maxComboCount = levelCompleteData.MaxComboCount;
            }
            SaveSystem.SaveLevelDatabase(levelSaveDatabase, LevelSaveFileName);
        }
    }
}