using System;
using System.Collections.Generic;
using Common.Timer;
using UnityEngine;

namespace Game.Manager
{
    public static class WorldTimeManager
    {
        private static Dictionary<string, SlotEntry> slots = new Dictionary<string, SlotEntry>();
        private static HashSet<string> activeLabels = new HashSet<string>();
        private static HashSet<string> pendingLabels = new HashSet<string>();
        private static List<string> helperLabels = new List<string>();
        
        private static float internalTime = 0;
        private static bool ticking;

        public static void Init()
        {
            TimerManager.Tick(Advance, 0, false, false);
        }

        public static void Add(string label, Action<long> callback)
        {
            if (ticking)
            {
                if (slots.TryGetValue(label, out var slot))
                {
                    Debug.LogWarning($"World time slot '{label}' exists");

                    switch (slot.Status)
                    {
                        case SlotStatus.Active:
                            activeLabels.Remove(label);
                            
                            slot.Status = SlotStatus.Pending;
                            slot.NextCallback = callback;
                            pendingLabels.Add(label);
                            break;
                        
                        case SlotStatus.Pending:
                            slot.NextCallback = callback;
                            break;
                        
                        default:
                            activeLabels.Remove(label);
                            
                            slot.Status = SlotStatus.Pending;
                            slot.NextCallback = callback;
                            pendingLabels.Add(label);
                            break;
                    }
                }
                else
                {
                    slot = new SlotEntry {Label = label, Status = SlotStatus.Pending, NextCallback = callback};
                    slots[label] = slot;
                    pendingLabels.Add(label);
                }
            }
            else
            {
                if (slots.TryGetValue(label, out var slot))
                {
                    Debug.LogWarning($"World time slot '{label}' exists");

                    switch (slot.Status)
                    {
                        case SlotStatus.Active:
                            slot.Callback = callback;
                            break;
                        
                        case SlotStatus.ActivePendingDelete:
                            slot.Status = SlotStatus.Active;
                            slot.Callback = callback;
                            break;
                        
                        default:
                            throw new InvalidOperationException("Should not in Pending Status");
                    }
                }
                else
                {
                    slot = new SlotEntry {Label = label, Callback = callback, Status = SlotStatus.Active};
                    slots[label] = slot;
                    activeLabels.Add(label);
                }
            }
        }

        public static void Remove(string label)
        {
            if (slots.TryGetValue(label, out var slot))
            {
                if (ticking)
                {
                    switch (slot.Status)
                    {
                        case SlotStatus.Pending:
                            pendingLabels.Remove(label);
                            RemoveSlot(slot);
                            break;
                        
                        case SlotStatus.Active:
                            activeLabels.Remove(label);
                            RemoveSlot(slot);
                            break;
                        
                        default:
                            break;
                    }
                }
                else
                {
                    switch (slot.Status)
                    {
                        case SlotStatus.Active:
                        case SlotStatus.ActivePendingDelete:
                            activeLabels.Remove(label);
                            RemoveSlot(slot);
                            break;
                    
                        default:
                            pendingLabels.Remove(label);
                            RemoveSlot(slot);
                            break;
                    }
                }
            }
        }

        private static void RemoveSlot(SlotEntry slot)
        {
            slots.Remove(slot.Label);
            slot.Dispose();
        }
        
        private static void Advance(float deltaTime)
        {
            internalTime += deltaTime;

            var seconds = 0;
            while (internalTime >= 1)
            {
                seconds++;
                internalTime = internalTime - 1;
            }

            if (seconds > 0)
            {
                if (activeLabels.Count > 0)
                {
                    ticking = true;
                
                    helperLabels.AddRange(activeLabels);
                    foreach (var label in helperLabels)
                    {
                        try
                        {
                            var slot = slots[label];
                            if (slot == null)
                            {
                                continue;
                            }
                            if (slot.Status == SlotStatus.ActivePendingDelete)
                            {
                                activeLabels.Remove(label);
                                RemoveSlot(slot);
                            }
                            else
                            {
                                slot.Callback?.Invoke(seconds);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                    }
                    helperLabels.Clear();

                    ticking = false;
                }
            
                if (pendingLabels.Count > 0)
                {
                    foreach (var label in pendingLabels)
                    {
                        var slot = slots[label];
                        slot.Status = SlotStatus.Active;
                        slot.Callback = slot.NextCallback;
                        slot.NextCallback = null;
                        activeLabels.Add(label);
                    }
                    pendingLabels.Clear();
                }
            }
        }

        private class SlotEntry
        {
            public string Label;
            public Action<long> Callback;
            public Action<long> NextCallback;
            public SlotStatus Status;

            public void Dispose()
            {
                Callback = null;
                NextCallback = null;
                Label = null;
            }
        }
        
        private enum SlotStatus
        {
            Active,
            Pending,
            ActivePendingDelete,
        }
    }
}