using System;
using System.Collections;
using System.Collections.Generic;
using Common.UI;
using Common.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scene
{
    public class SceneNavigator : MonoSingleton<SceneNavigator>
    {
        private List<SceneController> sceneControllers = new List<SceneController>();
        
        private Dictionary<SceneLoadCancellationToken, SceneLoadContext> tokenToContextDict =
            new Dictionary<SceneLoadCancellationToken, SceneLoadContext>();
        
        public SceneLoadCancellationToken GotoHome(DataBundle context = null)
        {
            return GotoScene("Home", context);
        }
        
        public SceneLoadCancellationToken GotoGame(DataBundle context = null)
        {
            return GotoScene("Game", context, false);
        }

        public SceneLoadCancellationToken GotoScene(string sceneName, DataBundle context = null, bool autoEnterScene = true)
        {
            foreach (var pair in tokenToContextDict)
            {
                if (pair.Value.SceneName.Equals(sceneName, StringComparison.InvariantCulture))
                {
                    return pair.Key;
                }
            }
            
            var loadContext = new SceneLoadContext {SceneName = sceneName};
            var cancellationToken = new SceneLoadCancellationToken(loadContext);
            tokenToContextDict[cancellationToken] = loadContext;
            LoadScene(loadContext, cancellationToken, context, autoEnterScene);
            return cancellationToken;
        }

        private void LoadScene(SceneLoadContext loadContext, SceneLoadCancellationToken cancellationToken, DataBundle context, bool autoEnterScene)
        {
            Toast.InvalidateCurrent();
            
            var container = LayerManager.Shared.GetLayer(LayerEnum.Scene);
            Addressables.InstantiateAsync($"{loadContext.SceneName}Scene", container).Completed += handle =>
            {
                if (SceneLoadStatus.Cancelled == loadContext.LoadStatus)
                {
                    GameObject.Destroy(handle.Result);
                }
                else
                {
                    handle.Result.transform.SetAsFirstSibling();
                    loadContext.LoadStatus = SceneLoadStatus.Created;
                    loadContext.SceneController = handle.Result.GetComponent<SceneController>();
                    loadContext.SceneController.OnCreate(context);
                    loadContext.Coroutine = StartCoroutine(WaitingForSceneReady(loadContext, cancellationToken, autoEnterScene));
                }
            };
        }

        private IEnumerator WaitingForSceneReady(SceneLoadContext loadContext, SceneLoadCancellationToken cancellationToken, bool autoEnterScene)
        {
            yield return null;
            yield return null;

            if (autoEnterScene)
            {
                EnterScene(loadContext, cancellationToken);
                loadContext.Dispose();
                loadContext.InvokeCompletionEvent(new SceneLoadCancellationCallback(false));
            }
            else
            {
                loadContext.LoadStatus = SceneLoadStatus.Ready;
                loadContext.InvokeCompletionEvent(new SceneLoadCancellationCallback(false, () =>
                {
                    EnterScene(loadContext, cancellationToken);
                    loadContext.Dispose();
                }));
            }
        }

        private void EnterScene(SceneLoadContext loadContext, SceneLoadCancellationToken cancellationToken)
        {
            while (sceneControllers.Count > 0)
            {
                var index = sceneControllers.Count - 1;
                var controller = sceneControllers[index];
                sceneControllers.RemoveAt(index);
                if (controller != null)
                {
                    controller.OnExit();
                    controller.OnDispose();
                    GameObject.Destroy(controller.gameObject);
                }
            }
            loadContext.LoadStatus = SceneLoadStatus.Completed;
            
            var sceneController = loadContext.SceneController;
            sceneControllers.Add(sceneController);
            sceneController.OnEnter();
            
            tokenToContextDict.Remove(cancellationToken);
        }

        public void CancelToken(SceneLoadCancellationToken cancellationToken)
        {
            if (null == cancellationToken || !cancellationToken.CanBeCancelled() || cancellationToken.IsCancellationRequested())
            {
                return;
            }
            
            if (!tokenToContextDict.TryGetValue(cancellationToken, out var loadContext))
            {
                return;
            }

            switch (loadContext.LoadStatus)
            {
                case SceneLoadStatus.Completed:
                    break;
                
                case SceneLoadStatus.Created:
                case SceneLoadStatus.Ready:
                    tokenToContextDict.Remove(cancellationToken);
                    StopCoroutine(loadContext.Coroutine);
                    GameObject.Destroy(loadContext.SceneController.gameObject);
                    loadContext.LoadStatus = SceneLoadStatus.Cancelled;
                    loadContext.InvokeCompletionEvent(new SceneLoadCancellationCallback(true));
                    loadContext.Dispose();
                    break;
                
                default:
                    tokenToContextDict.Remove(cancellationToken);
                    loadContext.LoadStatus = SceneLoadStatus.Cancelled;
                    loadContext.InvokeCompletionEvent(new SceneLoadCancellationCallback(true));
                    loadContext.Dispose();
                    break;
                
            }
        }
    }
}
