using System;
using System.Collections.Generic;
using Common.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.UI
{
    public class WindowManager : MonoSingleton<WindowManager>
    {
        [SerializeField] private RectTransform container;
        private UILayer uiLayer;
        
        private Dictionary<string, Window> windowSlots = new Dictionary<string, Window>();
        private List<string> windows = new List<string>();

        protected override void Init()
        {
            base.Init();
            uiLayer = GetComponent<UILayer>();
        }
        
        public void PushWindow<T>(DataBundle context = null) where T : UIWindow
        {
            var slotKey = typeof(T).FullName;
            if (!windowSlots.TryGetValue(slotKey, out var window))
            {
                window = new Window
                {
                    context = context,
                    sortingOrder = uiLayer.RentSortingLayerOrder(),
                };
                windowSlots[slotKey] = window;
                windows.Add(slotKey);
                OpenWindow<T>(window);
            }
            else
            {
                switch (window.state)
                {
                    case WindowState.Loading:
                        uiLayer.PutSortingLayerOrder(window.sortingOrder);
                        window.sortingOrder = uiLayer.RentSortingLayerOrder();
                        break;
                    
                    case WindowState.Destroying:
                        uiLayer.PutSortingLayerOrder(window.sortingOrder);
                        window.sortingOrder = uiLayer.RentSortingLayerOrder();
                        window.state = WindowState.Loading;
                        break;
                    
                    case WindowState.Destroyed:
                        OpenWindow<T>(window);
                        break;
                    
                    default:
                        break;
                }
            }
        }

        public void ReplaceWindow<T>(DataBundle context = null) where T : UIWindow
        {
            PopWindow(true);
            PushWindow<T>(context);
        }

        public void PopWindow(bool immediate = false)
        {
            if (windows.Count == 0)
            {
                return;
            }
            RemoveWindowAt(windows.Count - 1, immediate);
        }
        
        public void CloseWindow(UIWindow window, bool immediate = false)
        {
            var slotKey = window.GetType().FullName;
            var idx = windows.FindIndex(item => item.Equals(slotKey, StringComparison.InvariantCulture));
            if (idx >= 0)
            {
                RemoveWindowAt(idx, immediate);
            }
        }

        public void Clear()
        {
            while (windows.Count > 0)
            {
                RemoveWindowAt(windows.Count - 1, true);
            }
        }

        private void RemoveWindowAt(int index, bool immediate)
        {
            var slotKey = windows[index];
            var window = windowSlots[slotKey];
            switch (window.state)
            {
                case WindowState.Loading:
                    window.state = WindowState.Destroying;
                    break;
                
                case WindowState.Loaded:
                    window.state = WindowState.Destroyed;
                    windowSlots.Remove(slotKey);
                    windows.RemoveAt(index);

                    var windowAnimator = window.view.GetComponent<IWindowAnimator>();
                    if (null != windowAnimator && !immediate)
                    {
                        windowAnimator.PlayCloseAnimation(() =>
                        {
                            DestroyWindow(window);
                        });
                    }
                    else
                    {
                        window.view.OnCloseImmediately();
                        DestroyWindow(window);
                    }
                    break;
            }
        }

        private void DestroyWindow(Window window)
        {
            window.view.OnDispose();
            GameObject.Destroy(window.view.gameObject);
        }

        private void OpenWindow<T>(Window window) where T : UIWindow
        {
            window.state = WindowState.Loading;
            Addressables.InstantiateAsync(typeof(T).Name, LayerManager.Shared.GetLayer(LayerEnum.Popup)).Completed += handle =>
            {
                if (WindowState.Destroying == window.state)
                {
                    window.state = WindowState.Destroyed;
                    GameObject.Destroy(handle.Result);
                }
                else
                {
                    window.state = WindowState.Loaded;
                    window.view = handle.Result.GetComponent<T>();
                    window.view.SetSortingOrder(window.sortingOrder);
                    window.view.OnCreate(window.context);

                    var windowAnimator = window.view.GetComponent<IWindowAnimator>();
                    if (null != windowAnimator)
                    {
                        windowAnimator.PlayOpenAnimation();
                    }
                    else
                    {
                        window.view.OnOpenWillStart();
                        window.view.OnOpenDidFinish();
                    }
                }
            };
        }
    }

    class Window
    {
        public int sortingOrder;
        public DataBundle context;
        public UIWindow view;
        public WindowState state = WindowState.None;
    }

    enum WindowState
    {
        None,
        Loading,
        Loaded,
        Destroying,
        Destroyed,
    }
}