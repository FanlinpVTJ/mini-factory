using UnityEngine;
using WindowsManager.UI;
using Zenject;

namespace TabSystem
{
    public class TabActivateButton : AbstractButton
    {
        [SerializeField]
        private TabButton _targetTab;
        [SerializeField]
        private bool _isActivateDefaultButton = false;

        [Inject]
        private TabSystemManager _tabSystem;

        public override void OnButtonClick()
        {
            if (_isActivateDefaultButton)
            {
                _tabSystem.ActivateDefaultButton();
            }
            else
            {
                _tabSystem.SetTab(_targetTab);
            }
        }
    }
}
