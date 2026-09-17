using UnityEngine;
using WindowsManager.UI;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Кнопка закрытия окна
    /// </summary>
    public class CloseWindowButton : AbstractButton
    {
        [SerializeField] private WindowData _window;

        [Inject] private IWindowsManager _manager;

        public override void OnButtonClick()
        {
            _manager.CloseWindow(_window);
        }
    }
}