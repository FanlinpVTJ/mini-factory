using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "FpsLockInstaller", menuName = "Installers/FpsLockInstaller")]
public class FpsLockInstaller : ScriptableObjectInstaller<FpsLockInstaller>
{
    [Min(0)]
    [SerializeField]
    private int _targetFrameRate = 60;
    [SerializeField]
    private bool _useScreenUpdateRateIfItsHigher = true;

    public override void InstallBindings()
    {
        if (_useScreenUpdateRateIfItsHigher)
        {
            if (Screen.currentResolution.refreshRateRatio.value > _targetFrameRate)
                _targetFrameRate = Mathf.CeilToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }

        Container.BindInterfacesTo<FpsLock>().AsSingle().WithArguments(_targetFrameRate).NonLazy();
    }
}
