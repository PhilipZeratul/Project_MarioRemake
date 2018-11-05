using UnityEngine;
using Zenject;


public class PlayerController : MonoBehaviour
{
    ControllerManager controllerManager;

    [Inject]
    private void Init(ControllerManager _controllerManager)
    {
        controllerManager = _controllerManager;
    }

    private void Start()
    {
        controllerManager.Test();
    }
}
