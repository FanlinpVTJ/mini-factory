using UnityEngine;

namespace WindowsManager
{
    /// <summary>
    /// Данные об окне
    /// </summary>
    [CreateAssetMenu(menuName = "ToolsAndMechanics/Windows Manager/Window Data", fileName = "New Window")]
    public class WindowData : ScriptableObject
    {
        public Window WindowPrefab => _windowPrefab;

        [SerializeField] private Window _windowPrefab;
    }
}