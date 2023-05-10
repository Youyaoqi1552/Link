using System;

namespace Game.Event
{
    public static class GlobalEvent
    {
        public static event Action<bool> GamePause;
        public static event Action GameFinish;

        public static void InvokeGamePauseEvent(bool paused)
        {
            GamePause?.Invoke(paused);
        }
        
        public static void InvokeGameFinishEvent()
        {
            GameFinish?.Invoke();
        }
    }
}
