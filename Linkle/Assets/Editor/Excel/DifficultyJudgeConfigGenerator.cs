using System;
using System.IO;
using Game.Config;
using UnityEditor;
using UnityEngine;

public class DifficultyJudgeConfigGenerator
{
    [MenuItem("Tools/Generate Difficulty Judge Database")]
    public static void Run()
    {
        var selectPath = EditorUtility.OpenFolderPanel("Select Judge Configs folder", string.Empty, string.Empty);
        if (string.IsNullOrEmpty(selectPath))
        {
            return;
        }
        
        var database = ScriptableObject.CreateInstance<DifficultyConfigDatabase>();
        database.patternDifficultyConfigs = GetPatternDifficultyConfig(selectPath);
        database.robotDifficultyConfigs = GetRobotDifficultyConfigs(selectPath);
        database.difficultyJudgeConfigs = GetDifficultyJudgeConfig(selectPath);
        AssetDatabase.CreateAsset(database, "Assets/AssetsPackage/Configs/DifficultyDatabase.asset");
        AssetDatabase.Refresh();
        Debug.Log("Generate difficulty judge configs completed...");
    }

    private static PatternDifficultyConfig[] GetPatternDifficultyConfig(string parentPath)
    {
        var boardPath = Path.Combine(parentPath, "board.xlsx");
        var workBook = ExcelHelper.LoadWorkBook(boardPath);
        var sheet = workBook.GetSheetAt(0);
        
        var patternDifficultyConfigs = new PatternDifficultyConfig[sheet.LastRowNum - 2];
        for (var i = 3; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            var patternDifficultyConfig = new PatternDifficultyConfig();
            patternDifficultyConfig.id = Convert.ToInt32(row.GetCell(0).NumericCellValue);
            patternDifficultyConfig.theme = row.GetCell(1).StringCellValue.Trim();
            patternDifficultyConfig.difficultyLevel = Convert.ToInt32(row.GetCell(3).NumericCellValue);

            var tiles = row.GetCell(2).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
            patternDifficultyConfig.tiles = new int[tiles.Length];
            for (var j = 0; j < tiles.Length; j++)
            {
                patternDifficultyConfig.tiles[j] = Convert.ToInt32(tiles[j]);
            }
            patternDifficultyConfigs[i - 3] = patternDifficultyConfig;
        }
        workBook.Close();
        return patternDifficultyConfigs;
    }

    private static RobotDifficultyConfig[] GetRobotDifficultyConfigs(string parentPath)
    {
        var robotPath = Path.Combine(parentPath, "robot.xlsx");
        var workBook = ExcelHelper.LoadWorkBook(robotPath);
        var sheet = workBook.GetSheetAt(0);
        var robotDifficultyConfigs = new RobotDifficultyConfig[sheet.LastRowNum - 2];
        for (var i = 3; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            var robotDifficultyConfig = new RobotDifficultyConfig();
            robotDifficultyConfig.difficultyLevel = Convert.ToInt32(row.GetCell(0).NumericCellValue);

            var difficulties = row.GetCell(1).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
            robotDifficultyConfig.minBoardDifficultyLevel = Convert.ToInt32(difficulties[0]);
            robotDifficultyConfig.maxBoardDifficultyLevel = Convert.ToInt32(difficulties[1]);
            
            var reactTimes = row.GetCell(2).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
            robotDifficultyConfig.minReactTime = Convert.ToSingle(reactTimes[0]);
            robotDifficultyConfig.maxReactTime = Convert.ToSingle(reactTimes[1]);
            
            robotDifficultyConfigs[i - 3] = robotDifficultyConfig;
        }
        workBook.Close();
        return robotDifficultyConfigs;
    }

    private static DifficultyJudgeConfig[] GetDifficultyJudgeConfig(string parentPath)
    {
        var diffControllerPath = Path.Combine(parentPath, "diffController.xlsx");
        var workBook = ExcelHelper.LoadWorkBook(diffControllerPath);
        var sheet = workBook.GetSheetAt(0);
        var difficultyJudgeItemConfigs = new DifficultyJudgeConfig[sheet.LastRowNum - 2];
        for (var i = 3; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            var itemConfig = new DifficultyJudgeConfig();
            itemConfig.levelDifficulty = Convert.ToInt32(row.GetCell(1).NumericCellValue);
            itemConfig.patternDifficultyLevel = Convert.ToInt32(row.GetCell(3).NumericCellValue);

            var stateValues = row.GetCell(0).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
            itemConfig.minPlayerStateValue = Convert.ToInt32(stateValues[0]);
            itemConfig.maxPlayerStateValue = Convert.ToInt32(stateValues[1]);

            var robotDifficulties = row.GetCell(2).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
            itemConfig.minRobotDifficultyLevel = Convert.ToInt32(robotDifficulties[0]);
            itemConfig.maxRobotDifficultyLevel = Convert.ToInt32(robotDifficulties[1]);
            
            difficultyJudgeItemConfigs[i - 3] = itemConfig;
        }
        workBook.Close();
        return difficultyJudgeItemConfigs;
    }
}
