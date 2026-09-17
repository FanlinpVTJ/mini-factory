using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmartScroll
{
    public class SmartScrollElement : MonoBehaviour
    {
        public event Action OnDataUpdated;
        public event Action OnClear;

        private RectTransform _rt;
        private SmartScrollElementData _data;
        private bool _isVertical;

        public SmartScrollElementData Data { get => _data; }


        public void Init(bool isVertical)
        {
            _isVertical = isVertical;
            _rt = transform as RectTransform;
        }

        public void Clear()
        {
            OnClear?.Invoke();
            _data = null;
        }

        public void ApplyData(SmartScrollElementData data)
        {
            if (_isVertical)
            {
                _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, data.GetSize());
            }
            else
            {
                _rt.sizeDelta = new Vector2(data.GetSize(), _rt.sizeDelta.y);
            }
            _data = data;
            OnDataUpdated?.Invoke();
        }

        public virtual float GetDefaultSize(bool isVertical)
        {
            var rt = transform as RectTransform;
            return isVertical ? rt.rect.height : rt.rect.width;
        }

        public void UpdateData()
        {
            OnDataUpdated?.Invoke();
        }
    }
}
