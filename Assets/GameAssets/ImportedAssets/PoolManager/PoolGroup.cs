using UnityEngine;
using Zenject;

namespace PoolsUtility
{
    public class PoolGroup
    {
        public string PoolId { get => _poolId; }
        public PooledObject Prefab { get => _prefab; }
        public int InitialSize { get => _initialSize; }
        public int MaxSize { get => _maxSize; }
        public PoolExpandMethods PoolExpandMethod { get => _poolExpandMethod; }

        [SerializeField]
        private string _poolId;

        [SerializeField]
        private PooledObject _prefab;

        [SerializeField]
        private int _initialSize = 1;

        [SerializeField]
        private int _maxSize = -1;

        [SerializeField]
        private PoolExpandMethods _poolExpandMethod = PoolExpandMethods.Double;

        public PoolGroup(string poolId, PooledObject prefab, int initialSize = 1, int maxSize = -1, PoolExpandMethods poolExpandMethod = PoolExpandMethods.Double)
        {
            _poolId = poolId;
            _prefab = prefab;
            _initialSize = initialSize;
            _maxSize = maxSize;
            _poolExpandMethod = poolExpandMethod;
        }
    }
}