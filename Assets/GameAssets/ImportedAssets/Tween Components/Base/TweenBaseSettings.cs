using DG.Tweening;
using UnityEngine;

namespace TweenComponents
{
    [System.Serializable]
    public class TweenBaseSettings
    {
        public float StartDelay;
        [Min(0)] public float Duration;
        public Ease EaseType;
    }

}