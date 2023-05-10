using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.UI
{
    public class Toast
    {
        private static long currentGroup = 0;
        
        public static void MakeToast(string message, float duration = 2)
        {
            var group = currentGroup;
            Addressables.InstantiateAsync("Toast",  LayerManager.Shared.GetLayer(LayerEnum.Toast)).Completed += handle =>
            {
                if (group != currentGroup)
                {
                    GameObject.Destroy(handle.Result);
                }
                else
                {
                    var toast = handle.Result.GetComponent<ToastItem>();
                    toast.ShowWithMessage(message, duration);
                }
            };
        }

        public static void InvalidateCurrent()
        {
            if (currentGroup == long.MaxValue)
            {
                currentGroup = 0;
            }
            else
            {
                currentGroup++;
            }
        }
    }
}