using UnityEngine;
using Zenject;


public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public GameManager GameManager;
    public ControllerManager controllerManager;
    public GameObject player;


    public override void InstallBindings()
    {
        //Debug.Log("Global Installer Binding.");

        Container.Bind<GameManager>().FromInstance(Instantiate(GameManager)).AsSingle();
        Container.Bind<ControllerManager>().FromInstance(controllerManager).AsSingle();
        Container.Bind<GameObject>().WithId(Constants.InjectIDs.Player).FromInstance(Instantiate(player)).AsSingle();
    }
}
