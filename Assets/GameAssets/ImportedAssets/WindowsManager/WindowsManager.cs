using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Source: https://gitlab.com/syhodyb99/tools-and-mechanics
    /// Контроллер окон
    /// </summary>
    public class WindowsManager : IWindowsManager, IInitializable, IDisposable
    {
        public event Action<Window> OnNewWindowOpened;
        private DiContainer _instantiator;

        private Dictionary<WindowData, Window> _windowsDictionary = new Dictionary<WindowData, Window>();

        private List<Window> _windowsList = new List<Window>();
        private Window _lastWindow => _windowsList[_windowsList.Count - 1];
        private GameObject _canvas;
        private Transform _windowsContainer;
        private bool _inited;


        public WindowsManager(DiContainer instantiator, GameObject canvas)
        {
            _instantiator = instantiator;
            _canvas = canvas;
        }

        public void Initialize()
        {
            Init();
        }

        private void Init()
        {
            if (_inited)
                return;
            
            if (_canvas) _windowsContainer = _instantiator.InstantiatePrefab(_canvas).transform;
            else
            {
                _windowsContainer = new GameObject("Windows Container").transform;
            }
            _inited = true;
        }

        public Window OpenWindow(WindowData data, bool closeLastWindow = false)
        {
            if (!_inited) Init();

            if (_windowsDictionary.TryGetValue(data, out Window window))
            {
                if (closeLastWindow && _windowsList.Count > 0)
                {
                    _lastWindow.Close(window.Open);
                }
                else
                {
                    window.Open();
                }
            }
            else
            {
                window = CreateWindow(data);
                OpenWindow(data, closeLastWindow);
            }

            OnNewWindowOpened?.Invoke(window);
            return window;
        }

        public void OpenPreviousWindow(bool closeLastWindow = true)
        {
            if (!_inited) Init();

            if (_windowsList.Count > 1)
            {
                OpenWindow(_windowsList[^2].Data, closeLastWindow);
            }
        }

        public void CloseWindow(WindowData data)
        {
            if (!_inited) Init();

            if (_windowsDictionary.TryGetValue(data, out Window window))
            {
                window.Close();
            }
            else
            {
                Debug.LogWarning($"Окно с именем '{data.name}' не найдено");
            }
        }

        public void CloseWindows(List<WindowData> windows)
        {
            foreach (var w in windows)
            {
                CloseWindow(w);
            }
        }

        public void CloseAllWindows()
        {
            var keys = _windowsDictionary.Keys;

            foreach (var key in keys)
            {
                CloseWindow(key);
            }
            
        }

        public IEnumerable<Window> GetOpenedWindows()
        {
            return _windowsDictionary.Values.Where(item => item.IsOpen);
        }

        public Window GetWindowByData(WindowData data)
        {
            _windowsDictionary.TryGetValue(data, out Window result);
            return result;
        }

        public void AddWindowToList(Window window)
        {
            _windowsList.Add(window);
            OnNewWindowOpened?.Invoke(window);
        }

        public void RemoveWindowFromList(Window window)
        {
            _windowsList.Remove(window);
        }

        private Window CreateWindow(WindowData data)
        {
            Window window = _instantiator.InstantiatePrefabForComponent<Window>(data.WindowPrefab, _windowsContainer);
            window.Data = data;
            window.gameObject.name = data.name;
            _windowsDictionary.Add(data, window);
            return window;
        }

        public void Dispose()
        {
            GameObject.Destroy(_windowsContainer);
        }
    }
}