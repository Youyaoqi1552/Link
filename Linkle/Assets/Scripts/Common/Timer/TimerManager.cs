using System;
using System.Collections.Generic;
using Common.Utility;
using UnityEngine;

namespace Common.Timer
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        public static TimerToken After(float delay, Action onComplete, bool scaleTimeMode = true, int group = 0)
        {
            var obj = new TimerHelper();
            obj.Token = Shared.Add(0, deltaTime =>
            {
                if (obj.Completed)
                {
                    return;
                }
                
                obj.Completed = true;
                Shared.Remove(obj.Token);
                onComplete?.Invoke();
            }, delay, scaleTimeMode, false, @group);
            return obj.Token;
        }
        
        public static TimerToken Every(float interval, Action<float> callback, float delay = 0, bool scaleTimeMode = true, bool useAlignMode = false, int group = 0)
        {
            return Shared.Add(interval, callback, delay, scaleTimeMode, useAlignMode, group);
        }
        
        public static TimerToken Tick(Action<float> callback, float delay = 0, bool scaleTimeMode = true, bool useAlignMode = false, int group = 0)
        {
            return Shared.Add(0, callback, delay, scaleTimeMode, useAlignMode, group);
        }
        
        public static TimerToken Prior(float cutoff, Action onComplete = null, Action<float> onStep = null, bool scaleTimeMode = true, bool useAlignMode = false, int group = 0)
        {
            var obj = new TimerHelper();
            obj.Token = Shared.Add(0, deltaTime =>
            {
                if (obj.Completed)
                {
                    return;
                }
                
                obj.ElapsedTime += deltaTime;
                if (obj.ElapsedTime >= cutoff)
                {
                    obj.Completed = true;
                    Shared.Remove(obj.Token);
                    onComplete?.Invoke();
                }
                else
                {
                    onStep?.Invoke(deltaTime);
                }
            }, 0, scaleTimeMode, useAlignMode, @group);
            return obj.Token;
        }

        public static void Cancel(TimerToken token)
        {
            Shared.Remove(token);
        }
        
        public static void Pause(int group)
        {
            foreach (var pair in Shared.timerSlots)
            {
                if (pair.Key.Group == group)
                {
                    Shared.Pause(pair.Key);
                }
            }
        }
        
        public static void Resume(int group)
        {
            foreach (var pair in Shared.timerSlots)
            {
                if (pair.Key.Group == group)
                {
                    Shared.Resume(pair.Key);
                }
            }
        }
        
        public static void PauseAll()
        {
            foreach (var pair in Shared.timerSlots)
            {
                Shared.Pause(pair.Key);
            }
        }
        
        public static void ResumeAll()
        {
            foreach (var pair in Shared.timerSlots)
            {
                Shared.Resume(pair.Key);
            }
        }
        
        private Dictionary<TimerHandle, TimerData> timerSlots = new Dictionary<TimerHandle, TimerData>();
        private HashSet<TimerHandle> activeTimers = new HashSet<TimerHandle>();
        private HashSet<TimerHandle> pendingTimers = new HashSet<TimerHandle>();
        private HashSet<TimerHandle> pausedTimers = new HashSet<TimerHandle>();
        private List<TimerHandle> helperList = new List<TimerHandle>();
        
        private bool ticking;

        public TimerToken Add(float interval, Action<float> callback, float delay = 0, bool scaleTimeMode = true, bool useAlignMode = false, int group = 0)
        {
            var handle = TimerHandle.Next(group);
            var slot = new TimerData
            {
                Handle = handle,
                Interval = interval,
                Delay = delay,
                ScaleTimeMode = scaleTimeMode,
                UseAlignMode = useAlignMode
            };
            timerSlots[handle] = slot;
            
            if (ticking)
            {
                slot.Status = TimerStatus.Pending;
                slot.NextCallback = callback;
                pendingTimers.Add(handle);
            }
            else
            {
                slot.Status = TimerStatus.Active;
                slot.Callback = callback;
                activeTimers.Add(handle);
            }
            return new TimerToken(handle);
        }

        public void Remove(TimerToken token)
        {
            if (!Check(token))
            {
                return;
            }

            Remove(token.Handle);
        }
        
        private void Remove(TimerHandle handle)
        {
            if (!timerSlots.TryGetValue(handle, out var slot))
            {
                return;
            }
            
            if (ticking)
            {
                switch (slot.Status)
                {
                    case TimerStatus.Active:
                        activeTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                        
                    case TimerStatus.Paused:
                        pausedTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                        
                    case TimerStatus.Pending:
                        pendingTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                        
                    default:
                        break;
                }
            }
            else
            {
                switch (slot.Status)
                {
                    case TimerStatus.Active:
                    case TimerStatus.ActivePendingDelete:
                        activeTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                        
                    case TimerStatus.Paused:
                        pausedTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                        
                    case TimerStatus.Pending:
                        pendingTimers.Remove(handle);
                        RemoveTimerSlot(slot);
                        break;
                }
            }
        }

        public void Pause(TimerToken token)
        {
            if (!Check(token))
            {
                return;
            }

            Pause(token.Handle);
        }

        private void Pause(TimerHandle handle)
        {
            if (timerSlots.TryGetValue(handle, out var slot))
            {
                switch (slot.Status)
                {
                    case TimerStatus.Active:
                        activeTimers.Remove(handle);
                        
                        slot.Status = TimerStatus.Paused;
                        pausedTimers.Add(handle);
                        break;
                        
                    case TimerStatus.Pending:
                        pendingTimers.Remove(handle);
                        
                        slot.Status = TimerStatus.Paused;
                        slot.Callback = slot.NextCallback;
                        slot.NextCallback = null;
                        pausedTimers.Add(handle);
                        break;
                        
                    case TimerStatus.ActivePendingDelete:
                        break;
                            
                    case TimerStatus.Paused:
                        break;
                }
            }
        }
        
        public void Resume(TimerToken token)
        {
            if (!Check(token))
            {
                return;
            }
            Resume(token.Handle);
        }
        
        private void Resume(TimerHandle handle)
        {
            if (timerSlots.TryGetValue(handle, out var slot) && slot.Status == TimerStatus.Paused)
            {
                pausedTimers.Remove(handle);
                if (ticking)
                {
                    slot.Status = TimerStatus.Pending;
                    slot.NextCallback = slot.Callback;
                    slot.Callback = null;
                    pendingTimers.Add(handle);
                }
                else
                {
                    slot.Status = TimerStatus.Active;
                    activeTimers.Add(handle);
                }
            }
        }

        private bool Check(TimerToken token)
        {
            return token != null && token.Handle.IsValid;
        }

        private void RemoveTimerSlot(TimerData timerData)
        {
            timerSlots.Remove(timerData.Handle);
            timerData.Handle.Invalidate();
            timerData.Dispose();
        }

        private void AdvanceTime(float deltaTime, float unscaleDeltaTime)
        {
            if (activeTimers.Count > 0)
            {
                helperList.AddRange(activeTimers);
                foreach (var activeTimer in helperList)
                {
                    if (!timerSlots.TryGetValue(activeTimer, out var slot) || slot == null)
                    {
                        continue;
                    }
                    
                    switch (slot.Status)
                    {
                        case TimerStatus.ActivePendingDelete:
                            activeTimers.Remove(activeTimer);
                            RemoveTimerSlot(slot);
                            break;
                        
                        case TimerStatus.Active:
                            slot.Advance(slot.ScaleTimeMode ? deltaTime : unscaleDeltaTime);
                            break;
                        
                        default:
                            break;
                    }
                }
                
                helperList.Clear();
            }
            
            if (pendingTimers.Count > 0)
            {
                foreach (var pendingTimer in pendingTimers)
                {
                    var slot = timerSlots[pendingTimer];
                    slot.Status = TimerStatus.Active;
                    slot.Callback = slot.NextCallback;
                    slot.NextCallback = null;
                    activeTimers.Add(pendingTimer);
                }
                pendingTimers.Clear();
            }
        }
        
        private void Update()
        {
            ticking = true;
            AdvanceTime(Time.deltaTime, Time.unscaledDeltaTime);
            ticking = false;
        }

        private class TimerData
        {
            public TimerHandle Handle;
            public bool ScaleTimeMode;
            public bool UseAlignMode;
        
            public float Interval;
            public float Delay;
        
            public TimerStatus Status;
            public Action<float> Callback;
            public Action<float> NextCallback;

            private float elapsedTime;
            private bool alive = true;

            public void Advance(float deltaTime)
            {
                elapsedTime += deltaTime;

                var threshold = Delay + Interval;
                if (elapsedTime >= threshold)
                {
                    if (UseAlignMode && Interval > 0)
                    {
                        Delay = 0;

                        while (alive && elapsedTime >= threshold)
                        {
                            elapsedTime -= threshold;
                            threshold = Interval;
                            Callback?.Invoke(Interval);
                        }
                    }
                    else
                    {
                        Callback?.Invoke(elapsedTime);
                        elapsedTime = 0;
                        Delay = 0;
                    }
                }
            }

            public void Dispose()
            {
                Callback = null;
                NextCallback = null;
                alive = false;
            }
        }
        
        private enum TimerStatus
        {
            Active,
            Paused,
            Pending,
            ActivePendingDelete,
        }
        
        private class TimerHelper
        {
            public bool Completed;
            public float ElapsedTime;
            public TimerToken Token;
        }
    }
}