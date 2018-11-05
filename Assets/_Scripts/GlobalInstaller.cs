using UnityEngine;
using Zenject;


public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public ControllerManager controllerManager;


    public override void InstallBindings()
    {
        Debug.Log("Global Installer Binding.");

        Container.Bind<ControllerManager>().FromInstance(controllerManager).AsSingle();
    }
}
