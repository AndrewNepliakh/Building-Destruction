using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Managers
{
    public class UIManager : IUIManager, IInitializable
    {
        [Inject] private DiContainer _diContainer;

        private Window _currentWindow;
        private Popup _currentPopup;
        private Canvas _mainCanvas;

        private Dictionary<Type, IUIView> _viewsPool = new();

        public void Initialize()
        {
            var canvasGo = _diContainer.CreateEmptyGameObject("[MainCanvas]");
            var canvas = canvasGo.AddComponent<Canvas>();
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            
            scaler.referenceResolution = new Vector2(1080, 1920);

            _mainCanvas = canvas;
        }

        public async Task<T> ShowWindowWithDI<T>(UIViewArguments args = null)
            where T : Window
        {
            if (_currentWindow != null) _currentWindow.Hide(null);

            if (!_viewsPool.ContainsKey(typeof(T)))
            {
                var loader = new AssetsLoader();
                var newWindow =
                    await loader.InstantiateAssetWithDI<T>(typeof(T).ToString(), _diContainer, _mainCanvas.transform);
                _currentWindow = newWindow;
                _viewsPool.Add(typeof(T), _currentWindow);

                if (args == null) args = new UIViewArguments {AssetsLoader = loader};
                else args.AssetsLoader = loader;

                _currentWindow.Show(args);
            }
            else
            {
                if (_viewsPool.TryGetValue(typeof(T), out var uiView))
                {
                    if (args == null) args = new UIViewArguments();
                    _currentWindow = uiView as Window;
                    _currentWindow.Show(args);
                }
                else
                {
                    throw new NullReferenceException("UIManager's pool doesn't contain view of this type");
                }
            }

            _currentWindow.transform.localScale = Vector3.one;
            return (T) _currentWindow;
        }
        
        public async Task<T> ShowPopupWithDI<T>(UIViewArguments args = null)
            where T : Popup
        {
            if (_currentPopup != null) _currentPopup.Hide(null);
            
            if (!_viewsPool.ContainsKey(typeof(T)))
            {
                var loader = new AssetsLoader();
                var newPopup = await loader.InstantiateAssetWithDI<T>(typeof(T).ToString(), _diContainer, _mainCanvas.transform);
                _currentPopup = newPopup;
                _viewsPool.Add(typeof(T), _currentPopup);
                
                if (args == null) args = new UIViewArguments {AssetsLoader = loader};
                else args.AssetsLoader = loader;

                _currentPopup.Show(args);
            }
            else
            {
                if (_viewsPool.TryGetValue(typeof(T), out var uiView))
                {
                    if (args == null) args = new UIViewArguments();
                    _currentPopup = uiView as Popup;
                    _currentPopup.Show(args);
                }
                else
                {
                    throw new NullReferenceException("UIManager's pool doesn't contain view of this type");
                }
            }
            
            _currentPopup.transform.localScale = Vector3.one;
            return (T) _currentPopup;
        }

        public void HideWindow(UIViewArguments args)
        {
            _currentWindow.Hide(args);
        }

        public void HidePopup(UIViewArguments args)
        {
            _currentPopup.Hide(args);
        }

        public void UnloadWindows()
        {
            _currentPopup.AssetsLoader?.UnloadAsset();
            _currentWindow.AssetsLoader?.UnloadAsset();
        }
    }
}