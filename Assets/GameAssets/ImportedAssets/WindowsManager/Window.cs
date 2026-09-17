using System;
using UnityEngine;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Префаб окна
    /// </summary>
    public class Window : MonoBehaviour
    {
        [HideInInspector] public WindowData Data { get; internal set; }

        public bool IsOpen { get; protected set; }
        public virtual bool IsActive => gameObject.activeSelf;

        [Inject] protected IWindowsManager _windowsManager;

        protected IWindowAnimation _anim;
        protected Action _actionOnClose;

        protected virtual void Awake()
        {
            _anim = GetComponent<IWindowAnimation>();
        }

        /// <summary>
        /// Открыть окно
        /// </summary>
        internal virtual void Open()
        {
            if (IsOpen)
                return;

            gameObject.SetActive(true);
            IsOpen = true;
            _windowsManager.AddWindowToList(this);
            OpenInner();
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        internal virtual void Close(Action action = null)
        {
            if (!IsOpen)
                return;

            IsOpen = false;
            _actionOnClose = action;

            if (_anim != null)
            {
                _anim.Close(CloseInner);
            }
            else
            {
                CloseInner();
            }
        }

        protected virtual void OpenInner() { }

        protected virtual void CloseInner()
        {
            gameObject.SetActive(false);
            _windowsManager.RemoveWindowFromList(this);
            _actionOnClose?.Invoke();
        }
    }
}