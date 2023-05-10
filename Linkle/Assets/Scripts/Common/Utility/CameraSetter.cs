using UnityEngine;

namespace Common.Utility
{
    public class CameraSetter : MonoBehaviour
    {
        private void Awake()
        {
            var targetCamera = GetComponent<Camera>();
            targetCamera.orthographicSize = Screen.height / 200f;
        }
    }
}
