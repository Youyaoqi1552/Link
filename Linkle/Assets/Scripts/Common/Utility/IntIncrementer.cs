using System;
using UnityEngine;

namespace Common.Utility
{
    public class IntIncrementer
    {
        private int currentValue;
        private int targetValue;
        private int direction = 1;
        private float valueChangeRate = 150.0f;

        public event Action<int> OnValueChanged;

        public void SetChangeRate(float rate)
        {
            valueChangeRate = rate;
        }
        
        public int GetCurrentValue()
        {
            return currentValue;
        }

        public void SetValue(int value)
        {
            currentValue = targetValue = value;
            OnValueChanged?.Invoke(currentValue);
        }

        public void AddValue(int value, bool forceUpdateImmediately = false)
        {
            targetValue += value;

            if (forceUpdateImmediately)
            {
                currentValue = targetValue;
            }

            if (currentValue < targetValue)
            {
                direction = 1;
            }
            else if (currentValue > targetValue)
            {
                direction = -1;
            }
        }

        public void Tick(float deltaTime)
        {
            if (currentValue != targetValue)
            {
                currentValue += Mathf.FloorToInt(deltaTime * direction * valueChangeRate);
                if (direction < 0)
                {
                    if (currentValue < targetValue)
                    {
                        currentValue = targetValue;
                    }
                }
                else if (direction > 0)
                {
                    if (currentValue > targetValue)
                    {
                        currentValue = targetValue;
                    }
                }

                OnValueChanged?.Invoke(currentValue);
            }
        }
    }
}