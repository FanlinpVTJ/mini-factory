using UnityEngine;
using UnityEngine.UI;

namespace WindowsManager.UI
{
    /// <summary>
    /// Абстрактый класс кнопки
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class AbstractButton : MonoBehaviour
    {
        protected Button _btn;

        protected virtual void Awake()
        {
            _btn = GetComponent<Button>();
        }

        protected virtual void OnEnable()
        {
            _btn.onClick.AddListener(OnButtonClick);
        }

        protected virtual void OnDisable()
        {
            _btn.onClick.RemoveListener(OnButtonClick);
        }

        public abstract void OnButtonClick();
    }
}