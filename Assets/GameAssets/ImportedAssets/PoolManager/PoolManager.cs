using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PoolsUtility
{
    public class PoolManager 
    {
        [Inject]
        private IInstantiator _instantiator;

        private Transform DespawnRoot
        {
            get
            {
                if (_despawnRoot == null)
                {
                    var go = new GameObject("[POOL_DESPAWNED]");
                    Object.DontDestroyOnLoad(go);
                    _despawnRoot = go.transform;
                }
                return _despawnRoot;
            }
        }

        private Transform _despawnRoot;

        private Dictionary<string, PooledObjectPool> _pools = new Dictionary<string, PooledObjectPool>();

        private PooledObjectPool InstantiatePool(PooledObject prefab, string poolId, int initialSize = 1, int maxSize = int.MaxValue, PoolExpandMethods poolExpandMethods = PoolExpandMethods.Double)
        {
            MemoryPoolSettings settings = new MemoryPoolSettings(initialSize, maxSize, poolExpandMethods);
            return _instantiator.Instantiate<PooledObjectPool>(new object[] { settings, new PooledObjectFactory(prefab, _instantiator), poolId });
        }

        public PooledObjectPool GetPool (PoolGroup poolGroup)
        {
            if (!_pools.TryGetValue(poolGroup.PoolId, out PooledObjectPool pool))
            {
                pool = InitPool(poolGroup);
            }
            return pool;
        }

        public PooledObjectPool InitPool(PoolGroup poolGroup)
        {
            var pool = InstantiatePool(poolGroup.Prefab, poolGroup.PoolId, poolGroup.InitialSize, poolGroup.MaxSize > 0 ? poolGroup.MaxSize : int.MaxValue, poolGroup.PoolExpandMethod);
            _pools.Add(poolGroup.PoolId, pool);
            return pool;
        }

        //Without parent
        public PooledObject Instantiate(GameObject gameObject)
        {
            return Instantiate(gameObject.GetComponent<PooledObject>());
        }

        public PooledObject Instantiate(PooledObject pooledObject)
        {
            return InstantiateFromGroup(pooledObject.DefaultPoolGroup);
        }

        public PooledObject InstantiateFromGroup(PoolGroup poolGroup)
        {
            return InstantiateFromPool(GetPool(poolGroup));
        }

        public PooledObject InstantiateFromPool(PooledObjectPool pool)
        {
            return pool.Spawn();
        }

        //With parent
        public PooledObject Instantiate(GameObject gameObject, Transform parent)
        {
            return Instantiate(gameObject.GetComponent<PooledObject>(), parent);
        }

        public PooledObject Instantiate(PooledObject pooledObject, Transform parent)
        {
            return InstantiateFromGroup(pooledObject.DefaultPoolGroup, parent);
        }

        public PooledObject InstantiateFromGroup(PoolGroup group, Transform parent)
        {
            return InstantiateFromPool(GetPool(group), parent);
        }

        public PooledObject InstantiateFromPool(PooledObjectPool pool, Transform parent)
        {
            var obj = pool.Spawn();
            obj.transform.SetParent(parent, false);
            return obj;
        }

        public void Despawn(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<PooledObject>(out var pooled))
                Despawn(pooled);
            else
                Object.Destroy(gameObject);
        }

        public void Despawn(PooledObject pooledObject)
        {
            pooledObject.transform.SetParent(DespawnRoot, false);
            pooledObject.Pool.Despawn(pooledObject);
        }
    }
}
