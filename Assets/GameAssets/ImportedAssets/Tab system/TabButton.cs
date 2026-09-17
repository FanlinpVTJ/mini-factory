using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsManager.UI;
using Zenject;

namespace TabSystem
{
    public class TabButton : AbstractButton
    {
        public event Action OnActivated;

        [SerializeField] private Image _back;
        [SerializeField] private Image _lockedtabIcon;
        [SerializeField] private GameObject _text;
        [SerializeField] private TabVisualSettings _visualSettings = new TabVisualSettings();
        [SerializeField] private TabAnimationSettings _animationSettings = new TabAnimationSettings();
        [SerializeField] private List<GameObject> _objects;
        [SerializeField] private bool _isActiveTab;
        [SerializeField] private bool _isSelectedTab;
        [SerializeField] private RectTransform _rectTransform;

        private Sequence _sequence;

        [Inject] private TabSystemManager _system;

        public bool IsActiveTab => _isActiveTab;
        public bool IsSelectedTab => _isSelectedTab;

        public void SetActive(bool isSelected)
        {
            CheckSequence();

            _btn.interactable = _isActiveTab;

            _isSelectedTab = isSelected;

            if (isSelected)
            {
                OnActivated?.Invoke();
            }

            ApplyVisualState(isSelected);
            SetObjectsActive(isSelected);

            PlaySequence(isSelected);
        }

        private void PlaySequence(bool isActive)
        {
            CheckSequence();

            if (isActive)
            {
                _sequence.PlayForward();
            }
            else
            {
                _sequence.PlayBackwards();
            }
        }

        private void CheckSequence()
        {
            if (_sequence == null)
            {
                _sequence = DOTween.Sequence()
                    .SetAutoKill(false)
                    .SetUpdate(true)
                    .Pause();

                _sequence
                    .Append(_rectTransform.DOAnchorPosY(_animationSettings.SelectedTabYPosition, 0.15f).SetEase(Ease.Linear));
            }
        }

        public override void OnButtonClick()
        {
            if (_isActiveTab)
            {
                _system.SetTab(this);
            }
        }

        private void ApplyVisualState(bool isSelected)
        {
            _back.sprite = _visualSettings.GetBackgroundSprite(_isActiveTab, isSelected);
            _lockedtabIcon.gameObject.SetActive(!_isActiveTab);
            _text.SetActive(_isActiveTab);
        }

        private void SetObjectsActive(bool isSelected)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].SetActive(isSelected);
            }
        }
    }
}
