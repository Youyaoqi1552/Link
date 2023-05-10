using UnityEngine;

namespace Common.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T mInstance = null;

        public static T Shared
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (mInstance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        mInstance = go.AddComponent<T>();
                        var parent = GameObject.Find("Boot");
                        if (parent == null)
                        {
                            parent = new GameObject("Boot");
                        }
                        go.transform.SetParent(parent.transform);
                    }
                    mInstance.Init();
                }
                return mInstance;
            }
        }

        private void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this as T;
                Init();
            }
            else if (mInstance != this)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        protected virtual void Init()
        {

        }
        
        protected virtual void Dispose()
        {

        }
        
        public void DestroySelf()
        {
            Dispose();
            mInstance = null;
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}