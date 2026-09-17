using UnityEngine;

namespace MiniFactory.Configuration
{
    [CreateAssetMenu(fileName = "FactoryConfiguration", menuName = "Mini Factory/Factory Configuration")]
    public sealed class FactoryConfigurationAsset : ScriptableObject, IFactoryConfigurationSource
    {
        [SerializeField] private string _currencyIdentifier;

        [SerializeField] private MachineConfiguration[] _machines = new MachineConfiguration[]
        {
            new MachineConfiguration("machine_1", true, 1, 0, 10, 1.15, 500),
            new MachineConfiguration("machine_2", false, 5, 50, 50, 1.15, 500),
            new MachineConfiguration("machine_3", false, 20, 250, 200, 1.15, 500)
        };

        [Header("Boost")]
        [SerializeField] private bool _boostEnabled = true;
        [SerializeField, Min(1)] private double _boostDurationSeconds = 30;
        [SerializeField, Min(1)] private double _boostMultiplier = 2;

        [Header("Offline Production")]
        [SerializeField, Min(0)] private double _maximumOfflineSeconds = 28800;

        public FactoryConfiguration Load()
        {
            FactoryConfiguration configuration = new FactoryConfiguration(_currencyIdentifier, _machines, _boostEnabled,
                _boostDurationSeconds, _boostMultiplier, _maximumOfflineSeconds);
            configuration.Validate();
            MachineConfiguration[] machines = new MachineConfiguration[_machines.Length];

            for (int i = 0; i < _machines.Length; i++)
            {
                MachineConfiguration machine = _machines[i];
                machines[i] = new MachineConfiguration(machine.Identifier, machine.InitiallyUnlocked,
                    machine.ProductionPerSecond, machine.UnlockPrice, machine.BaseUpgradePrice,
                    machine.UpgradePriceMultiplier, machine.MaximumLevel);
            }

            FactoryConfiguration result = new FactoryConfiguration(_currencyIdentifier, machines, _boostEnabled,
                _boostDurationSeconds, _boostMultiplier, _maximumOfflineSeconds);
            return result;
        }
    }
}
