using MiniFactory.Configuration;
using MiniFactory.Economy;
using UnityEngine;
using Zenject;

namespace MiniFactory.Installation
{
    public sealed class FactoryInstaller : MonoInstaller
    {
        [SerializeField] private FactoryConfigurationAsset _configuration;

        public override void InstallBindings()
        {
            IFactoryConfigurationSource source = _configuration;
            FactoryConfiguration configuration = source.Load();
            Container.Bind<IFactoryConfigurationSource>().FromInstance(source).AsSingle();
            Container.Bind<FactoryConfiguration>().FromInstance(configuration).AsSingle();
            Container.Bind<FactoryEconomy>().AsSingle().NonLazy();
        }
    }
}
