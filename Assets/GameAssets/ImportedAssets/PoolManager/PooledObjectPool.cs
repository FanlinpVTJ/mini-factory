using UnityEngine;
using Zenject;

namespace PoolsUtility
{
    public class PooledObjectPool: MonoMemoryPool<PooledObject>
    {
        private string _poolId;
        private Transform _transformGroupRoot;

        public PooledObjectPool(string poolId, IInstantiator instantiator)
        {
            _poolId = poolId;            
            _transformGroupRoot = instantiator.CreateEmptyGameObject(poolId).transform;
        }

        protected override void OnCreated(PooledObject item)
        {
            base.OnCreated(item);
            item.Pool = this;
            item.Spawn();
            item.transform.SetParent(_transformGroupRoot);
        }

        protected override void OnDespawned(PooledObject item)
        {
            base.OnDespawned(item);
            item.transform.SetParent(_transformGroupRoot);
        }

        public new void Dispose()
        {
            base.Dispose();
            UnityEngine.Object.Destroy(_transformGroupRoot.gameObject);
        }
    }
}
