using UnityEngine;
using UnityEngine.UI;

namespace WindowsManager.UI
{
    /// <summary>
    /// Абстрактый класс слайдера
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public abstract class AbstractSlider : MonoBehaviour
    {
        protected Slider _slider;

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        protected virtual void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        public abstract void OnValueChanged(float value);
    }
}