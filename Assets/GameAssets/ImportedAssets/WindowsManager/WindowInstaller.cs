using UnityEngine;
using Zenject;

namespace WindowsManager
{
    /// <summary>
    /// Инсталлер для окна. Используется в GameObjectContext
    /// </summary>
    [RequireComponent(typeof(Window))]
    public class WindowInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            if (TryGetComponent<Window>(out var window))
            {
                var windowType = window.GetType();
                if (windowType.GetInterfaces().Length > 0) Container.BindInterfacesTo(windowType).FromInstance(window).AsSingle();
                Container.Bind<Window>().FromInstance(window).AsSingle();

                //if (typeof(Window) != window.GetType())
                //{
                //    Container.BindInstance(window).AsSingle();
                //}
            }
        }
    }
}