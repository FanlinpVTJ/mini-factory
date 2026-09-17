using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSaver
{
    public class ScriptableSaveDataBase : ScriptableObject 
    {
        internal new void SetDirty()
        {
#if UNITY_EDITOR

            UnityEditor.EditorUtility.SetDirty(this);

#endif
        }
    }

	public class ScriptableSaveDataGeneric<T> : ScriptableSaveDataBase where T : new()
	{
        #region Fields

        [SerializeField]
        private T _saveObject;

        #endregion

        #region Properties

        public T SaveObject { get => _saveObject; internal set => _saveObject = value; }

        #endregion
    }
}