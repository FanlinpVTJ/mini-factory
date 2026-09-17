using System;
using System.Collections;
using UnityEngine;

namespace WindowsManager
{
    /// <summary>
    /// Реализация интерфейса для анимаций
    /// закрытия/открытия окон
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationWindow : MonoBehaviour, IWindowAnimation
    {
        [SerializeField] private string _animOpenKey = "IsOpen";
        [SerializeField] private float _closeAnimTime;

        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Open();
        }

        public void Open()
        {
            _anim.SetBool(_animOpenKey, true);
        }

        public void Close(Action action)
        {
            _anim.SetBool(_animOpenKey, false);
            StartCoroutine(CloseWindowWithDelay(action));
        }

        private IEnumerator CloseWindowWithDelay(Action action)
        {
            yield return new WaitForSecondsRealtime(_closeAnimTime);
            action?.Invoke();
        }
    }
}