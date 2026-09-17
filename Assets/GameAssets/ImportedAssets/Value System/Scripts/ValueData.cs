using UnityEngine;

namespace ValueSystem
{
	[CreateAssetMenu(menuName = "Game/Value Data")]
	public class ValueData : ScriptableObject
	{
		public string Id => _id;

		public bool IsInfinite => _isInfinite;
		public float MaxCount => _maxCount;
		public float MinCount => _minCount;
		public float DefaulCount => _defaultCount;

		public Sprite Icon => _icon;
		public Color Color => _color;
		public string DisplayName => _displayName;

		[SerializeField]
		private string _id;

		[Space]
		[SerializeField]
		private float _minCount = 0;

		[Header("If not infinite => limited by Max Count")]
		[SerializeField]
		private bool _isInfinite = true;
		[SerializeField]
		private float _maxCount = 0;

		[Space]
		[Header("UI Settings")]
		[SerializeField]
		private Sprite _icon;
		[SerializeField]
		private Color _color = Color.white;
		[SerializeField]
		private string _displayName;

		[Space]
		[Header("Default settings value")]
		[SerializeField]
		private float _defaultCount;

#if UNITY_EDITOR
		private void OnValidate()
		{
			var formattedName = name.Replace(" ", "");
			if (Id == string.Empty || Id != formattedName)
			{
				_id = formattedName;
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}
#endif
	}
}

