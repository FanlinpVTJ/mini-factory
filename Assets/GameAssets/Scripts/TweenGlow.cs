using System;
using DG.Tweening;
using UnityEngine;

public class TweenGlow : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _delayStart;
    [Space]
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    [SerializeField] private Vector3 _roatateStart;
    [SerializeField] private Vector3 _roatateEnd;
    [Space]
    [SerializeField] private AnimationCurve _curve;

    private Tween _glowTween;
    private RectTransform _rectTransform;

    public float Duration => _duration;

    protected virtual void Awake()
    {
        ResetState();
        BuildGlowTween();
    }

    protected virtual void OnEnable()
    {
        PlayGlow();
    }

    protected virtual void OnDisable()
    {
        PauseGlow();
    }

    protected virtual void OnDestroy()
    {
        _glowTween.KillIfActive();
        _glowTween = null;
    }

    public virtual void PlayGlow()
    {
        ResetState();
        _glowTween.Restart();
    }

    public virtual void PauseGlow()
    {
        _glowTween.Pause();
        _glowTween.Rewind();
        ResetState();
    }

    protected virtual void BuildGlowTween()
    {
        _glowTween.KillIfActive();
        _rectTransform.DOKill();

        _glowTween = DOTween.Sequence()
            .AppendInterval(_delayStart)
            .AppendAction(ResetState)
            .Append(_rectTransform.DOAnchorPos(_end, _duration))
            .Join(_rectTransform.DORotate(_roatateEnd, _duration, RotateMode.LocalAxisAdd))
            .SetEase(_curve)
            .SetLoops(-1, LoopType.Restart)
            .SetAutoKill(false)
            .SetUpdate(true)
            .Pause();
    }

    protected virtual void ResetState()
    {
        if (!_rectTransform)
            _rectTransform = (RectTransform)transform;

        _rectTransform.anchoredPosition = _start;
        _rectTransform.rotation = Quaternion.Euler(_roatateStart);
    }
}

public static class SequnceExtensions
{
    public static Sequence AppendAction(this Sequence s, Action action)
    {
        s.AppendCallback(new TweenCallback(() => action?.Invoke()));
        return s;
    }
}

public static class TweenExtension
{
    /// <summary>
    /// Kills tween if it`s active. If it`s null or non active then do nothing.
    /// </summary>
    /// <param name="tween"></param>
    public static void KillIfActive(this Tween tween, bool complete = false)
    {
        if (tween.IsActive())
            tween.Kill(complete);
    }
}
