using DG.Tweening;
using UnityEngine;

namespace WindowsManager.UI
{
    public class ButtonPressAnimation : AbstractButton
    {
        private Tween _animation;
        public override void OnButtonClick()
        {
            if (_animation.IsActive())
                _animation.Kill(true);

            _animation = transform.DOPunchScale(-Vector3.one * 0.15f, 0.15f)
                .SetUpdate(true)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject);
        }
    }
}

