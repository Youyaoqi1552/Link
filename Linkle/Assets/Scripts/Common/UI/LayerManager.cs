using System;
using System.Collections;
using System.Collections.Generic;
using Common.Utility;
using UnityEngine;

namespace Common.UI
{
    public enum LayerEnum
    {
        Scene, Popup, Toast
    }

    [Serializable]
    public class LayerSlot
    {
        public LayerEnum layer;
        public RectTransform container;
    }
    
    public class LayerManager : MonoSingleton<LayerManager>
    {
        [SerializeField] private LayerSlot[] slots;

        private Dictionary<int, LayerSlot> layers = new Dictionary<int, LayerSlot>();
        
        protected override void Init()
        {
            base.Init();

            foreach (var slot in slots)
            {
                layers[(int) slot.layer] = slot;
            }
        }
        
        public RectTransform GetLayer(LayerEnum layer)
        {
            if (layers.TryGetValue((int) layer, out var slot))
            {
                return slot.container;
            }
            return null;
        }
    }
}
