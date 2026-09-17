using System;
using UnityEngine;

namespace TabSystem
{
    [Serializable]
    public struct TabAnimationSettings
    {
        [SerializeField] private float _selectedTabYPosition;
        [SerializeField] private float _unselectedTabYPosition;

        public float SelectedTabYPosition => _selectedTabYPosition;
        public float UnselectedTabYPosition => _unselectedTabYPosition;
    }
}
