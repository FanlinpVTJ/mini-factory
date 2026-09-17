using WindowsManager.UI;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Кнопка открытия окна
    /// </summary>
    public class OpenPreviousWindowButton : AbstractButton
    {
        [Inject] private IWindowsManager _manager;

        protected override void OnEnable()
        {
            base.OnEnable();
            _btn.interactable = true;
        }

        public override void OnButtonClick()
        {
            _btn.interactable = false;
            _manager.OpenPreviousWindow();
        }
    }
}