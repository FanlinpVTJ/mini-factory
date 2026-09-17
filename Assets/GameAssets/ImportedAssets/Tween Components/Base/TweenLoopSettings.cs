using DG.Tweening;
using UnityEngine;

namespace TweenComponents
{
    [System.Serializable]
    public class TweenLoopSettings
    {
        [Tooltip("-1 means infinite loop. 0 means no loops")]
        [Min(-1)] public int LoopCount = 0;

        public LoopType LoopType = LoopType.Yoyo;
    }
}
