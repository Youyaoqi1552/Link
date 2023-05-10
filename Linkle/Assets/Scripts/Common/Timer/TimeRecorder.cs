namespace Common.Timer
{
    public class TimeRecorder
    {
        private float elapsedTime;
        private TimerToken timerToken;
        
        public float ElapsedTime => elapsedTime;

        public void Dispose()
        {
            Stop();
        }
        
        public void Start()
        {
            elapsedTime = 0;
            timerToken = TimerManager.Tick(OnTimer, 0, false);
        }

        public void Stop()
        {
            TimerManager.Cancel(timerToken);
            timerToken = null;
        }
        
        public void Reset()
        {
            elapsedTime = 0;
        }

        public void Pause()
        {
            TimerManager.Shared.Pause(timerToken);
        }
        
        public void Resume()
        {
            TimerManager.Shared.Resume(timerToken);
        }
        
        private void OnTimer(float deltaTime)
        {
            elapsedTime += deltaTime;
        }
    }
}