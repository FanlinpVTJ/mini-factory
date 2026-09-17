using UnityEngine;
using Zenject;

namespace PoolsUtility
{
    public class PooledObjectFactory : IFactory<PooledObject>
    {
        private readonly PooledObject _prefab;
        private readonly IInstantiator _instantiator;

        public PooledObjectFactory(PooledObject prefab, IInstantiator instantiator)
        {
            _prefab = prefab;
            _instantiator = instantiator;
        }

        public PooledObject Create()
        {
            return _instantiator.InstantiatePrefabForComponent<PooledObject>(_prefab);
        }
    }
}
