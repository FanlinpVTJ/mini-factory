using TMPro;
using UnityEngine;

namespace WindowsManager.UI
{
    /// <summary>
    /// Абстрактый класс текста
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class AbstractText : MonoBehaviour
    {
        protected TMP_Text _text;

        protected virtual void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }
    }
}