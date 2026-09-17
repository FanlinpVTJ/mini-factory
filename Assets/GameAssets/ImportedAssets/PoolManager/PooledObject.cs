using UnityEngine;
using Zenject;

namespace PoolsUtility
{
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// Default group on PoolManager.Instantiate
        /// </summary>
        public PoolGroup DefaultPoolGroup { get => _defaultPoolGroup; set => _defaultPoolGroup = value; }
        internal PooledObjectPool Pool { get => _pool; set => _pool = value; }

        //[SerializeField, Header("Default group on PoolManager.Instantiate")]
        private PoolGroup _defaultPoolGroup;

        private PooledObjectPool _pool;

        public virtual void Spawn()
        {
        }

        public virtual void Despawn()
        {
            if (_pool!= null) 
            { 
                _pool.Despawn(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void Despawn(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<PooledObject>(out var pooledObject))
            {
                pooledObject.Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}