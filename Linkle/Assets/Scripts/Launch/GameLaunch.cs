using Game.Config;
using Game.Manager;
using Game.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class GameLaunch : MonoBehaviour
{
    private int totalAssetCount;
    private void Start()
    {
        Input.multiTouchEnabled = false;
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
        Addressables.InitializeAsync().Completed += OnAddressableInitialized;
    }

    private void OnAddressableInitialized(AsyncOperationHandle<IResourceLocator> asyncOperationHandle)
    {
        totalAssetCount = 2;

        DifficultyConfigDatabase difficultyConfigDatabase = null;
        LevelDatabase levelDatabase = null;
        
        var globalConfigsAsyncOperation = Addressables.LoadAssetsAsync<ScriptableObject>("globalconfigs", null);
        globalConfigsAsyncOperation.Completed += handle =>
        {
            foreach (var config in handle.Result)
            {
                switch (config.name)
                {
                    case "GameSettings":
                        Global.GameSettingsConfig = config as GameSettingsConfig;
                        break;
                    
                    case "DifficultyDatabase":
                        difficultyConfigDatabase = config as DifficultyConfigDatabase;
                        break;
                    
                    case "LevelDatabase":
                        levelDatabase = config as LevelDatabase;
                        break;
                    
                    default:
                        break;
                }
            }
            
            LevelManager.Init(levelDatabase, difficultyConfigDatabase);
            OnPreloadAssetLoaded();
        };
        
        Addressables.LoadAssetsAsync<Texture2D>("avatars", null).Completed += handle =>
        {
            AvatarManager.SetLocalAvatars(handle.Result);
            OnPreloadAssetLoaded();
        };
    }
    
    private void OnPreloadAssetLoaded()
    {
        totalAssetCount--;
        if (totalAssetCount <= 0)
        {
            OnAllPreloadAssetsLoaded();
        }
    }

    private void OnAllPreloadAssetsLoaded()
    {
        WorldTimeManager.Init();
        UserManager.LoadPlayerData();
        SceneNavigator.Shared.GotoHome();
    }
}