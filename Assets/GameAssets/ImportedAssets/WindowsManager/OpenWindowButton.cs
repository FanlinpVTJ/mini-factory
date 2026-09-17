using UnityEngine;
using WindowsManager.UI;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Кнопка открытия окна
    /// </summary>
    public class OpenWindowButton : AbstractButton
    {
        [SerializeField] private WindowData _window;
        [SerializeField] private bool _closeCurrentWindow = true;

        [Inject] private IWindowsManager _manager;

        protected override void OnEnable()
        {
            base.OnEnable();
            _btn.interactable = true;
        }

        public override void OnButtonClick()
        {
            if (_closeCurrentWindow)
                _btn.interactable = false;
            _manager.OpenWindow(_window, _closeCurrentWindow);
        }
    }
}