using System;
using System.Collections.Generic;
using System.IO;
using Game.Config;
using Game.Link;
using NPOI.SS.UserModel;
using UnityEditor;
using UnityEngine;

public static class LevelGenerator
{
    [MenuItem("Tools/Generate Level Database")]
    public static void GenerateLevels()
    {
        var selectPath = EditorUtility.OpenFolderPanel("Select Levels folder", string.Empty, string.Empty);
        if (string.IsNullOrEmpty(selectPath))
        {
            return;
        }
        
        var rootPath = new DirectoryInfo(selectPath);
        var dictHelper = new Dictionary<int, List<int>>();
        var pairsHelper = new Dictionary<int, int>();
        var levelConfigs = new List<LevelConfig>();
        foreach (var subDirInfo in rootPath.GetDirectories())
        {
            var boardConfigs = new List<BoardConfig>();
            foreach (var path in subDirInfo.GetFiles())
            {
                var excelName = Path.GetFileNameWithoutExtension(path.Name);
                if(string.IsNullOrEmpty(excelName) || excelName.StartsWith("~$")) continue;
                
                var boardConfig = new BoardConfig();
                var workBook = ExcelHelper.LoadWorkBook(path.FullName);
                var sheet = workBook.GetSheet("Sheet1");
                
                var isValid = false;
                var size = Vector2Int.zero;
                var tileCount = 0;
                for (var i = sheet.LastRowNum; i >= sheet.FirstRowNum; i--)
                {
                    var rowLength = 0;
                    var row = sheet.GetRow(i);
                    var rows = new List<int>();
                    foreach (var cell in row.Cells)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Numeric:
                            {
                                var tag = Convert.ToInt32(cell.NumericCellValue);
                                tileCount++;
                                if (pairsHelper.TryGetValue(tag, out var count))
                                {
                                    count--;
                                    if (count > 0)
                                    {
                                        pairsHelper[tag] = count;
                                    }
                                    else
                                    {
                                        pairsHelper.Remove(tag);
                                    }
                                }
                                else
                                {
                                    pairsHelper[tag] = 1;
                                }
                                rows.Add(tag);
                                rowLength = cell.ColumnIndex + 1;
                                isValid = true;
                                break;
                            }
                            default:
                                rows.Add(-1);
                                break;
                        }
                    }
                    dictHelper[i] = rows;

                    if (size.x < rowLength)
                    {
                        size.x = rowLength;
                    }
                    
                    if (isValid)
                    {
                        size.y++;
                    }
                }

                if (0 != tileCount % 2)
                {
                    throw new Exception($"Tile Count cannot be divided by 2, at {path.FullName}");
                }

                foreach (var pair in pairsHelper)
                {
                    if (pair.Value != 0)
                    {
                        throw new Exception($"There exists tile mismatch with tag '{pair.Key}' at {path.FullName}");
                    }
                }

                var tileConfigs = new List<TileConfig>();
                for (var row = 0; row < size.y; row++)
                {
                    var rows = dictHelper[row];
                    for (var col = 0; col < size.x; col++)
                    {
                        var tileConfig = new TileConfig {tag = col < rows.Count ? rows[col] : -1};
                        tileConfigs.Add(tileConfig);
                    }
                }

                boardConfig.tileConfigs = tileConfigs.ToArray();
                boardConfig.width = size.x;
                boardConfig.height = size.y;
                boardConfig.maxPairs = tileCount / 2;
                boardConfig.fallingConfigs = new[]
                {
                    new FallingConfig {direction = (int) Direction.Down, step = 1},
                    new FallingConfig {direction = (int) Direction.Right, step = 1},
                    new FallingConfig {direction = (int) Direction.Up, step = 1},
                    new FallingConfig {direction = (int) Direction.Left, step = 1},
                };

                var nextSheet = workBook.GetSheet("Sheet2");
                if (null != nextSheet)
                {
                    var difficulties = nextSheet.GetRow(0).GetCell(0).StringCellValue.Trim(ExcelHelper.TrimChars).Split(',');
                    var totalDifficulty = 0;
                    foreach (var difficulty in difficulties)
                    {
                        totalDifficulty += Convert.ToInt32(difficulty);
                    }
                    boardConfig.difficulty = totalDifficulty / difficulties.Length;
                }
                boardConfigs.Add(boardConfig);

                dictHelper.Clear();
                pairsHelper.Clear();
                workBook.Close();
            }

            var levelConfig = new LevelConfig
            {
                boardConfigs = boardConfigs.ToArray()
            };
            levelConfigs.Add(levelConfig);
        }
        var levelDatabase = ScriptableObject.CreateInstance<LevelDatabase>();
        levelDatabase.levels = levelConfigs.ToArray();
        
        var targetPath = Path.Combine(Application.dataPath, "AssetsPackage", "Configs");
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        AssetDatabase.CreateAsset(levelDatabase, $"Assets/AssetsPackage/Configs/LevelDatabase.asset");
        AssetDatabase.Refresh();
        Debug.Log("Generate levels completed...");
    }
}
