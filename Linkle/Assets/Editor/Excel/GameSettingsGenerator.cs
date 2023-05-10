using System;
using Game.Config;
using UnityEditor;
using UnityEngine;

public class GameSettingsGenerator
{
    [MenuItem("Tools/Generate Game Settings")]
    public static void GenerateLevels()
    {
        var selectPath = EditorUtility.OpenFilePanel("Select Game Settings file", string.Empty, string.Empty);
        if (string.IsNullOrEmpty(selectPath))
        {
            return;
        }
        
        var workBook = ExcelHelper.LoadWorkBook(selectPath);
        var sheet = workBook.GetSheetAt(0);
        
        var config = ScriptableObject.CreateInstance<GameSettingsConfig>();
        config.rankBonusScores = new int[2];
        for (var i = 3; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            var propertyCell = row.GetCell(0);
            var valueCell = row.GetCell(2);
            switch (propertyCell.StringCellValue)
            {
                case "score":
                    config.basicTileRewardScore = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
                case "doubleScore":
                    config.doubleScoreDuration = Convert.ToSingle(valueCell.NumericCellValue / 1000);
                    break;
                case "comboTime":
                    config.comboJudgementInterval = Convert.ToSingle(valueCell.NumericCellValue / 1000);
                    break;
                case "recallLevels":
                    config.recallLevels = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
                case "baseStatusValue":
                    config.baseStatusValue = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
                case "winTimes":
                    config.winTimes = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
                case "first":
                    config.rankBonusScores[0] = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
                case "last":
                    config.rankBonusScores[1] = Convert.ToInt32(valueCell.NumericCellValue);
                    break;
            }
        }
        workBook.Close();

        config.lifeRestoreDuration = 30 * 60;
        config.refillLifeCostCredits = 100;
        AssetDatabase.CreateAsset(config, "Assets/AssetsPackage/Configs/GameSettings.asset");
        AssetDatabase.Refresh();
        Debug.Log("Generate game settings completed...");
    }
}