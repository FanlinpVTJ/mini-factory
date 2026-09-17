using System;
using UnityEngine;

namespace TabSystem
{
    [Serializable]
    public class TabVisualSettings
    {
        [SerializeField] private Sprite _selectedBackgroundSprite;
        [SerializeField] private Sprite _unselectedBackgroundSprite;
        [SerializeField] private Sprite _disabledBackgroundSprite;

        public Sprite GetBackgroundSprite(bool isTabActive, bool isTabSelected)
        {
            if (!isTabActive)
            {
                return _disabledBackgroundSprite;
            }

            return isTabSelected ? _selectedBackgroundSprite : _unselectedBackgroundSprite;
        }
    }
}
