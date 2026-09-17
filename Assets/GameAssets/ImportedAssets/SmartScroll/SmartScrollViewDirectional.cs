using UnityEngine;
using UnityEngine.UI;

namespace SmartScroll
{
    public class SmartScrollViewDirectional : SmartScrollView
    {
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        protected override float GetDefaultSize()
        {
            return _prefab.GetDefaultSize(_isVertical);
        }

        protected override float GetElementSpaceSize(SmartScrollElementData data)
        {
            return data.GetSize() + _layoutGroup.spacing;
        }
        protected override float GetSpacing()
        {
            return _layoutGroup.spacing;
        }
    }
}
