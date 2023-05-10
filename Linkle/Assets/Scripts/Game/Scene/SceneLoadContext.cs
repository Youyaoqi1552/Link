using System;
using UnityEngine;

namespace Game.Scene
{
    public enum SceneLoadStatus
    {
        Pending,
        Created,
        Ready,
        Completed,
        Cancelled,
    }

    public class SceneLoadContext
    {
        public SceneLoadStatus LoadStatus = SceneLoadStatus.Pending;
            
        public string SceneName;
        public SceneController SceneController;
        public Coroutine Coroutine;

        public event Action<SceneLoadCancellationCallback> Completed;

        public void InvokeCompletionEvent(SceneLoadCancellationCallback callback)
        {
            Completed?.Invoke(callback);
            Completed = null;
        }

        public void Dispose()
        {
            SceneController = null;
            Coroutine = null;
            SceneName = null;
        }
    }
}