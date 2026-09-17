using UnityEngine;
using Zenject;

namespace MiniFactory.Production
{
    public sealed class FactoryLifecycle : MonoBehaviour
    {
        private bool _isStarted;
        private bool _isPaused;
        private bool _hasFocus;

        [Inject] private FactorySession _session;

        private void Start()
        {
            _hasFocus = Application.isFocused;
            _isStarted = true;
            UpdateSuspension();
        }

        private void OnApplicationPause(bool paused)
        {
            _isPaused = paused;
            UpdateSuspension();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
            UpdateSuspension();
        }

        private void OnApplicationQuit()
        {
            if (_isStarted)
            {
                _session.SetSuspended(true);
            }
        }

        private void OnDestroy()
        {
            if (_isStarted)
            {
                _session.SetSuspended(true);
            }
        }

        private void UpdateSuspension()
        {
            if (_isStarted)
            {
                _session.SetSuspended(_isPaused || !_hasFocus);
            }
        }
    }
}
