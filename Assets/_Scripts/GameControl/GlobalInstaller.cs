using UnityEngine;
using Zenject;


public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public ControllerManager controllerManager;
    public GameObject player;


    public override void InstallBindings()
    {
        //Debug.Log("Global Installer Binding.");

        Container.Bind<ControllerManager>().FromInstance(controllerManager).AsSingle();
        Container.Bind<GameObject>().WithId(Constants.InjectIDs.Player).FromInstance(Instantiate(player)).AsSingle();
    }
}
