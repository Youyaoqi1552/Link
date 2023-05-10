using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GameThemeGenerator
{
    [MenuItem("Tools/Generate Game Themes")]
    public static void GenerateGameThemes()
    {
        var targetPath = Path.Combine(Application.dataPath, "AssetsPackage", "Themes");
        var rootPath = Path.Combine(Application.dataPath, "AssetsPackage", "UI", "Atlas", "Themes");
        var root = new DirectoryInfo(rootPath);
        foreach (var fileInfo in root.GetFiles("*.png"))
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath($"Assets/AssetsPackage/UI/Atlas/Themes/{fileInfo.Name}");
            if (assets.Length <= 1)
            {
                continue;
            }

            var themeConfig = ScriptableObject.CreateInstance<GameThemeConfig>();
            themeConfig.sprites = new Sprite[assets.Length - 1];
            for (var i = 1; i < assets.Length; i++)
            {
                themeConfig.sprites[i - 1] = assets[i] as Sprite;
            }
            
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            var themeName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            AssetDatabase.CreateAsset(themeConfig, $"Assets/AssetsPackage/Themes/{themeName}.asset");
        }
        AssetDatabase.Refresh();
    }
}
