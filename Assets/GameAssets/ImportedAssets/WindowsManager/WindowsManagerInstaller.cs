using UnityEngine;
using Zenject;

namespace WindowsManager
{
    [CreateAssetMenu(menuName = "ToolsAndMechanics/Windows Manager/Installer")]
    public class WindowsManagerInstaller : ScriptableObjectInstaller
    {
        [SerializeField, Tooltip("Optional")] private GameObject _container;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<WindowsManager>().AsSingle().WithArguments(_container);
        }
    }
}