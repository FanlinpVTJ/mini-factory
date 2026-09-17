using UnityEngine;
using UnityEngine.UI;

namespace SmartScroll
{
    public class SmartScrollViewGrid : SmartScrollView
    {
        [SerializeField]
        private GridLayoutGroup _grid;

        private float Spacing => _isVertical ? _grid.spacing.y : _grid.spacing.x;

        public override bool CanChangeElementSize => false;

        protected void Awake()
        {
            if ((!_isVertical || _grid.constraint != GridLayoutGroup.Constraint.FixedColumnCount) && (_isVertical || _grid.constraint != GridLayoutGroup.Constraint.FixedRowCount))
            {
                Debug.LogError("Grid layout group must have constraint according to scroll type!");
            }
        }

        protected override float GetDefaultSize()
        {
            return _isVertical ? _grid.cellSize.y : _grid.cellSize.x;
        }

        protected override float GetElementSpaceSize(SmartScrollElementData data)
        {
            return data.Index % _grid.constraintCount == _grid.constraintCount - 1 ? data.GetSize() + Spacing : 0;
        }

        protected override float GetSpacing()
        {
            return Spacing;
        }
    }
}
