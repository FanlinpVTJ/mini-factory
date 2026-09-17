using DG.Tweening;
using UnityEngine;

namespace TweenComponents.Base
{
    public static class TweenExtension
    {
        public static Tween ApplyBaseSettings(this Tween tween, TweenBase settings, GameObject targetObject)
        {
            if (tween == null) return null;

            tween.onKill += settings.OnExecutionCompleted;

            return tween.SetDelay(settings.Delay)
                .SetLoops(settings.LoopCount, settings.LoopType)
                .SetUpdate(settings.IgnoreTimeScale)
                .SetEase(settings.EaseType)
                .SetLink(targetObject)
                .SetAutoKill(true);
        }
    }
}

