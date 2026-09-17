using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

namespace SmartScroll
{
	public abstract class SmartScrollView : MonoBehaviour
	{
		[Inject]
		private IInstantiator _instantiator;

		[SerializeField]
		protected SmartScrollElement _prefab;
		[SerializeField]
		private HorizontalOrVerticalLayoutGroup _contentLayoutGroup;
		[SerializeField]
		private ScrollRect _scrollRect;
		[SerializeField]
		private RectTransform _elements;
		[SerializeField]
		private LayoutElement _paddingStart;
		[SerializeField]
		private LayoutElement _paddingEnd;
		[SerializeField]
		private int _defaultElementCount;
		[SerializeField]
		private bool _elementsAlwaysActive;

		private RectTransform _content => _scrollRect.content;
		private List<SmartScrollElementData> _elementDatas = new List<SmartScrollElementData>();
		private Dictionary<int, SmartScrollElement> _spawnedElements = new Dictionary<int, SmartScrollElement>();
		private Stack<SmartScrollElement> _pool = new Stack<SmartScrollElement>();

		protected bool _isVertical => _scrollRect.vertical;

		public IReadOnlyList<SmartScrollElementData> ElementDatas { get => _elementDatas; }
		public IReadOnlyDictionary<int, SmartScrollElement> SpawnedElements { get => _spawnedElements; }
		public virtual bool CanChangeElementSize => true;

		protected float ContentPaddingStartSize
		{
			get
			{
				if (_isVertical)
				{
					return _contentLayoutGroup.padding.top;
				}
				else
				{
					return _contentLayoutGroup.padding.left;
				}
			}
		}

		protected float ContentPaddingEndSize
		{
			get
			{
				if (_isVertical)
				{
					return _contentLayoutGroup.padding.bottom;
				}
				else
				{
					return _contentLayoutGroup.padding.right;
				}
			}
		}

		public void SetInteractable(bool isScrolled)
		{
			if (!TryGetComponent<CanvasGroup>(out var canvasGroup))
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}

			canvasGroup.blocksRaycasts = isScrolled;
		}

		protected abstract float GetDefaultSize();
		protected abstract float GetElementSpaceSize(SmartScrollElementData data);
		protected abstract float GetSpacing();

		protected virtual void Start()
		{
			if (_defaultElementCount > 0)
			{
				CreateElements(_defaultElementCount);
			}
		}
		protected virtual void Update()
		{
			if (!_elementsAlwaysActive)
				UpdateView();
		}

		public void CreateElements(SmartScrollElement prefab, int count)
		{
			_prefab = prefab;
			CreateElements(count);
		}

		public void CreateElements(int count)
		{
			float defaultSize = GetDefaultSize();
			for (int i = 0; i < count; i++)
			{
				_elementDatas.Add(new SmartScrollElementData(i, defaultSize, CanChangeElementSize));
			}

			UpdateView();
		}

		public float GetElementPosition(int index, bool includePadding = true)
		{
			float position = includePadding ? ContentPaddingStartSize : 0;
			for (int i = 0; i < index && i < _elementDatas.Count; i++)
			{
				position += GetElementSpaceSize(_elementDatas[i]);
			}
			return position;
		}

		[EasyButtons.Button]
		public void ScrollToElement(int index, float offset = 0)
		{
			float contentSize = ContentPaddingStartSize + ContentPaddingEndSize + _elementDatas.Sum((elementData) => GetElementSpaceSize(elementData)) - GetSpacing();
			float position = _isVertical ?
				Mathf.Clamp(GetElementPosition(index, false), 0f, contentSize - (_scrollRect.transform as RectTransform).rect.size.y) :
				Mathf.Clamp(GetElementPosition(index, false), 0f, contentSize - (_scrollRect.transform as RectTransform).rect.size.x);
			position += offset;
			_content.anchoredPosition = _isVertical ? new Vector2(_content.anchoredPosition.x, position) : new Vector2(-position, _content.anchoredPosition.y);
			UpdateView();
		}


		public Tween ScrollToElementAnimate(int index, float offset, float duration, bool isVertical)
		{
			float contentSize = ContentPaddingStartSize + ContentPaddingEndSize + _elementDatas.Sum((elementData) => GetElementSpaceSize(elementData)) - GetSpacing();
			float position = isVertical ?
				Mathf.Clamp(GetElementPosition(index, false), 0f, contentSize - (_scrollRect.transform as RectTransform).rect.size.y) :
				Mathf.Clamp(GetElementPosition(index, false), 0f, contentSize - (_scrollRect.transform as RectTransform).rect.size.x);
			position += offset;
			Vector2 targetPosition = isVertical ? new Vector2(_content.anchoredPosition.x, position) : new Vector2(-position, _content.anchoredPosition.y);

			return _content.DOAnchorPos(targetPosition, duration).OnUpdate(() => UpdateView());
		}

		public virtual void UpdateView()
		{
			float contentPosition = _isVertical ? _content.anchoredPosition.y : -_content.anchoredPosition.x;
			float viewportSize = _isVertical ? _scrollRect.viewport.rect.size.y : _scrollRect.viewport.rect.size.x;
			bool foundFirstVisibleElement = false;
			float offsetStart = 0;
			float offsetEnd = 0;
			float combinedSize = _isVertical ? _contentLayoutGroup.padding.top : _contentLayoutGroup.padding.left;
			int visiblesElementsCount = 0;
			bool IsElementVisible(SmartScrollElementData smartScrollElementData)
			{
				if (_elementsAlwaysActive)
					return true;
				float elementPosition = combinedSize;
				return elementPosition < contentPosition + viewportSize && elementPosition + smartScrollElementData.GetSize() > contentPosition;
			}
			for (int i = 0; i < _elementDatas.Count; i++)
			{
				SmartScrollElementData data = _elementDatas[i];
				bool isVisible = IsElementVisible(data);
				if (isVisible != data.IsActive)
				{
					if (isVisible)
					{
						var element = Spawn();
						_spawnedElements.Add(i, element);
						element.ApplyData(data);
						//element.transform.SetParent(_elements);
						element.transform.SetSiblingIndex(visiblesElementsCount);
					}
					else
					{
						Despawn(_spawnedElements[i]);
						_spawnedElements.Remove(i);
					}
					data.IsActive = isVisible;

				}
				if (isVisible)
					visiblesElementsCount++;
				float elementSpaceSize = GetElementSpaceSize(data);
				if (!foundFirstVisibleElement)
				{
					if (!isVisible)
					{
						offsetStart += elementSpaceSize;
					}
					else
					{
						foundFirstVisibleElement = true;
					}
				}
				else
				{
					if (!isVisible)
					{
						offsetEnd += elementSpaceSize;
					}
				}
				combinedSize += elementSpaceSize;
			}
			if (_isVertical)
			{
				_paddingStart.preferredHeight = offsetStart;
				_paddingEnd.preferredHeight = offsetEnd;
			}
			else
			{
				_paddingStart.preferredWidth = offsetStart;
				_paddingEnd.preferredWidth = offsetEnd;
			}
		}

		public void Clear()
		{
			foreach (var element in _spawnedElements.Values)
			{
				element.Data.IsActive = false;
				Despawn(element);
			}
			_spawnedElements.Clear();
			_elementDatas.Clear();
		}

		protected SmartScrollElement Spawn()
		{
			if (_pool.TryPop(out SmartScrollElement instance))
			{
				instance.gameObject.SetActive(true);
				return instance;
			}
			instance = _instantiator.InstantiatePrefabForComponent<SmartScrollElement>(_prefab, _elements);
			instance.Init(_isVertical);
			instance.gameObject.name += _spawnedElements.Count;

			return instance;
		}

		protected void Despawn(SmartScrollElement instance)
		{
			instance.transform.SetAsLastSibling();
			instance.gameObject.SetActive(false);
			instance.Clear();
			_pool.Push(instance);
		}
	}
}
