using UnityEngine;
using UnityEngine.UI;

namespace WindowsManager.UI
{
    /// <summary>
    /// Абстрактый класс тоггла
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public abstract class AbstractToggle : MonoBehaviour
    {
        protected Toggle _toggle;

        protected virtual void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        protected virtual void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        public abstract void OnValueChanged(bool value);
    }
}