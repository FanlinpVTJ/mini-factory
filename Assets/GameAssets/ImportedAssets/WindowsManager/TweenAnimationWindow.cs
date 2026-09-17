using System;
using System.Collections;
using System.Linq;
using TweenComponents.Base;
using UnityEngine;

namespace WindowsManager
{
    /// <summary>
    /// Реализация интерфейса для анимаций
    /// закрытия/открытия окон через твины
    /// </summary>
    public class TweenAnimationWindow : MonoBehaviour, IWindowAnimation
    {
        [SerializeField] private bool _openStraight = true;
        [SerializeField] private bool _closeStraight = false;
        [SerializeField] private TweenBase[] _openTweens;
        [SerializeField] private TweenBase[] _closeTweens;

        private void OnEnable()
        {
            Open();
        }

        public void Open()
        {
            if (_openTweens.Length > 0)
                AnimTweens(_openTweens, _openStraight);
        }

        public void Close(Action action)
        {
            if (_closeTweens.Length > 0)
            {
                AnimTweens(_closeTweens, _closeStraight);
                StartCoroutine(CloseWindowWithDelay(action));
            }
            else
            {
                action?.Invoke();
            }
        }

        private IEnumerator CloseWindowWithDelay(Action action)
        {
            yield return new WaitUntil(() => _closeTweens.All(t => t.IsCompleted) || _closeTweens.Length == 0);
            action?.Invoke();
        }

        private void AnimTweens(TweenBase[] tweens, bool straight)
        {
            foreach (var t in tweens)
            {
                t.Execute(straight);
            }
        }
    }
}