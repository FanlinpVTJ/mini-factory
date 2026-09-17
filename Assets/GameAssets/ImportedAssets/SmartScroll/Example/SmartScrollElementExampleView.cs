using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmartScroll.Example
{
    public class SmartScrollElementExampleView : MonoBehaviour
    {
        [SerializeField]
        private SmartScrollElement _smartScrollElement;

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private GameObject _expandedGroup;

        [SerializeField]
        private float _expandDuration = 0.5f;


        [SerializeField]
        private float _expandedHeight;

        [SerializeField]
        private bool _canChangeSize = false;


        private float _originalHeight;
        private Tween _tween;
        private RectTransform _rt;

        private void Awake()
        {
            _rt = transform as RectTransform;
            _originalHeight = _rt.rect.size.y;
            _smartScrollElement.OnDataUpdated += UpdateView;
            _smartScrollElement.OnClear += Clear;
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _smartScrollElement.OnDataUpdated -= UpdateView;
            _smartScrollElement.OnClear -= Clear;
            _button.onClick.RemoveListener(OnClick);
        }

        private void UpdateView()
        {
            _text.text = _smartScrollElement.Data.Index.ToString();
            switch (_smartScrollElement.Data.State)
            {
                //0 - not expanded
                case 0:
                    _expandedGroup.SetActive(false);
                    break;
                //1 - expanded
                case 1:
                    _expandedGroup.SetActive(true);
                    break;
            }
        }

        private void Clear()
        {
            _tween.Kill();
            if (_canChangeSize)
                switch (_smartScrollElement.Data.State)
                {
                    case 0:
                        _smartScrollElement.Data.Size = _originalHeight;
                        break;
                    case 1:
                        _smartScrollElement.Data.Size = _expandedHeight;
                        break;
                }
        }
        private void OnClick()
        {
            if (_canChangeSize)
                switch (_smartScrollElement.Data.State)
                {
                    case 0:
                        AnimateExpand(true);
                        _smartScrollElement.Data.State = 1;
                        break;
                    case 1:
                        AnimateExpand(false);
                        _smartScrollElement.Data.State = 0;
                        break;
                }
        }

        private void AnimateExpand(bool isForward)
        {
            _tween?.Kill();
            if (isForward)
            {
                _tween = DOVirtual.Float(0, 1, _expandDuration, (v) =>
                {
                    _smartScrollElement.Data.Size = Mathf.Lerp(_originalHeight, _expandedHeight, v);
                    _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _smartScrollElement.Data.Size);
                }).SetLink(gameObject).OnComplete(() =>
                {
                    _expandedGroup.SetActive(true);
                });

            }
            else
            {
                _expandedGroup.SetActive(false);
                _tween = DOVirtual.Float(1, 0, _expandDuration, (v) =>
                {
                    _smartScrollElement.Data.Size = Mathf.Lerp(_originalHeight, _expandedHeight, v);
                    _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _smartScrollElement.Data.Size);
                }).SetLink(gameObject);

            }
        }
    }
}
