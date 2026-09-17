using UnityEngine;
using Zenject;

namespace PoolsUtility 
{
    public class PoolManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PoolManager>().AsSingle();
        }
    } 
}