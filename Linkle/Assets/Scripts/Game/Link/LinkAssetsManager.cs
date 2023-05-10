using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Link
{
    public class LinkAssetsManager : MonoBehaviour
    {
        public static LinkAssetsManager Current;
        
        [SerializeField] private SpriteAtlasSheet spriteAtlas;
        [SerializeField] private LinkTileSelection tileSelectionPrefab;
        
        private List<GameThemeConfig> gameThemeConfigs;
        private int totalAssetCount;
        private bool disposed;
        private int currentTheme = 0;

        public event Action onAllAssetsLoaded;

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            Current = null;
        }

        public void LoadAssets(LinkLevelInfo levelInfo)
        {
            totalAssetCount = 1;

            var themeAsyncOperation = Addressables.LoadAssetsAsync<GameThemeConfig>("gamethemes", null);
            themeAsyncOperation.Completed += handle =>
            {
                if (disposed)
                {
                }
                else
                {
                    gameThemeConfigs = new List<GameThemeConfig>();
                    foreach (var patternDifficultyConfig in levelInfo.patternDifficultyConfigs)
                    {
                        gameThemeConfigs.AddRange(handle.Result.Where(x => string.Equals(x.name, patternDifficultyConfig.theme, StringComparison.InvariantCulture)));
                    }
                    OnAssetLoaded();
                }
            };
        }

        private void OnAssetLoaded()
        {
            totalAssetCount--;
            if (totalAssetCount <= 0)
            {
                onAllAssetsLoaded?.Invoke();
            }
        }

        public void Dispose()
        {
            disposed = true;
        }
        
        public Sprite GetAtlasSprite(string spriteName)
        {
            return spriteAtlas.GetSprite(spriteName);
        }

        public Sprite GetThemeSprite(int tileTag)
        {
            return gameThemeConfigs[currentTheme].GetSpriteAt(tileTag - 1);
        }
        
        public Sprite GetThemeSprite(string assetName)
        {
            return gameThemeConfigs[currentTheme].GetSprite(assetName);
        }

        public void ChangeTheme()
        {
            currentTheme++;
            if (currentTheme >= gameThemeConfigs.Count)
            {
                currentTheme = 0;
            }
        }
        
        public LinkTileSelection AllocateTileSelection(RectTransform parent)
        {
            var tileSelection = Instantiate(tileSelectionPrefab, parent);
            return tileSelection;
        }
        
        public void ReleaseTileSelection(LinkTileSelection tileSelection)
        {
            tileSelection.OnRecycle();
            Destroy(tileSelection.gameObject);
        }
    }
}
