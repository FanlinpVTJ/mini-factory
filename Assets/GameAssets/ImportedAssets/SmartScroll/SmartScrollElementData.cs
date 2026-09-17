using UnityEngine;

namespace SmartScroll
{
    [System.Serializable]
    public class SmartScrollElementData
    { 
        private int _index;
        private float _size;
        private readonly bool _canChangeElementSize;
        private int _state;
        private bool _isActive;

        public int Index => _index;
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public int State { get => _state; set => _state = value; }
        public float Size { get => _size; 
            set
            {
                if (_canChangeElementSize)
                    _size = value;
                else
                    Debug.LogError("This smart scroll type doesn't support dynamic size");
            }
        }

        public SmartScrollElementData(int index, float size, bool canChangeElementSize)
        {
            _index = index;
            _size = size;
            _canChangeElementSize = canChangeElementSize;
        }

        public virtual float GetSize()
        {
            return _size;
        }
    }
}
