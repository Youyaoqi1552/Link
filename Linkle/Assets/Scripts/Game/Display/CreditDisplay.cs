using System.Collections;
using System.Collections.Generic;
using Common.Utility;
using TMPro;
using UnityEngine;

namespace Game.Display
{
    public class CreditDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;
        
        private IntIncrementer incrementer;

        private void Awake()
        {
            incrementer = new IntIncrementer();
            incrementer.OnValueChanged += OnAmountChanged;
        }

        private void OnDestroy()
        {
            incrementer.OnValueChanged -= OnAmountChanged;
            incrementer = null;
        }
        
        private void OnAmountChanged(int value)
        {
            amountText.text = value.ToString();
        }

        public void SetAmount(int value)
        {
            incrementer.SetValue(value);
        }

        public void Add(int value, bool forceImmediateUpdate = false)
        {
            incrementer.AddValue(-value, forceImmediateUpdate);
        }

        private void Update()
        {
            incrementer.Tick(Time.deltaTime);
        }
    }
}
