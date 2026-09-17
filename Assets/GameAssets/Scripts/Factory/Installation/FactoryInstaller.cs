using MiniFactory.Configuration;
using MiniFactory.Economy;
using MiniFactory.Persistence;
using MiniFactory.Production;
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
            Container.Bind<IFactoryClock>().To<SystemFactoryClock>().AsSingle();
            Container.Bind<IFactoryProgressStorage>().To<GameSaverFactoryProgressStorage>().AsSingle();
            Container.Bind<FactoryProduction>().AsSingle();
            Container.BindInterfacesAndSelfTo<FactorySession>().AsSingle();
        }
    }
}
