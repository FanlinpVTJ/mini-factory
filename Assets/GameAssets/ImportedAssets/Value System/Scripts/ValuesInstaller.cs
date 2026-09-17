using UnityEngine;
using ValueSystem.Base;
using ValueSystem.Save;
using Zenject;

namespace ValueSystem.Zenject
{
    [CreateAssetMenu(fileName = "Values Installer", menuName = "Installers/Values Installer")]
    public class ValuesInstaller : ScriptableObjectInstaller<ValuesInstaller>
    {
        [SerializeField]
        private ValueData[] _valueDatas;

        public ValueData[] Values => _valueDatas;

        public override void InstallBindings()
        {
            ValuesSave valuesSave = new ValuesSave(null);
            Container.Bind<IValueSystem>().To<ValueSystemLogic>().AsSingle().WithArguments(_valueDatas, valuesSave).NonLazy();
        }
    }
}
