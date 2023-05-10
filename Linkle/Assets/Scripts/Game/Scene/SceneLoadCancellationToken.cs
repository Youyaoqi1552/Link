using System;

namespace Game.Scene
{
    public class SceneLoadCancellationCallback
    {
        public readonly bool Cancelled;

        public readonly Action Callback;
        
        public SceneLoadCancellationCallback(bool cancelled, Action callback = null)
        {
            Cancelled = cancelled;
            Callback = callback;
        }
    }
    
    public class SceneLoadCancellationToken
    {
        private SceneLoadContext loadContext;

        public event Action<SceneLoadCancellationCallback> completed
        {
            add => loadContext.Completed += value;
            remove => loadContext.Completed -= value;
        }
        
        public SceneLoadCancellationToken(SceneLoadContext context)
        {
            loadContext = context;
        }

        public bool CanBeCancelled()
        {
            return SceneLoadStatus.Completed !=  loadContext.LoadStatus;
        }

        public bool IsCancellationRequested()
        {
            return SceneLoadStatus.Cancelled ==  loadContext.LoadStatus;
        }
    }
}