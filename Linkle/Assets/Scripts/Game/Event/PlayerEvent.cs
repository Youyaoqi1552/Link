using System;

namespace Game.Event
{
    public static class PlayerEvent
    {
        public static event Action CreditsChanged;
        
        public static event Action LivesChanged;
        public static event Action<long> LifeRestoring;

        public static void InvokeCreditsChangedEvent()
        {
            CreditsChanged?.Invoke();
        }
        
        public static void InvokeLivesChangedEvent()
        {
            LivesChanged?.Invoke();
        }
        
        public static void InvokeLifeRestoringEvent(long time)
        {
            LifeRestoring?.Invoke(time);
        }
    }
}