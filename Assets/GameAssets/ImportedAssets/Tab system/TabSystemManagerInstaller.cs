using UnityEngine;
using Zenject;

namespace TabSystem
{
    [RequireComponent(typeof(TabSystemManager))]
    public class TabSystemManagerInstaller : MonoInstaller
    {
        [SerializeField] private TabSystemManager _tabSystemManager;

        public override void InstallBindings()
        {
            Container.Bind<TabSystemManager>().FromInstance(_tabSystemManager).AsSingle();
        }
    }
}
