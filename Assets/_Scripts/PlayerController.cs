using UnityEngine;
using Zenject;


public class PlayerController : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float jumpSpeed = 20f;
    public float maxJumpHoldTime = 0.5f;

    private PhysicsObject physicsObject;
    private ControllerManager controllerManager;
    private float jumpStartTime;

    [Inject]
    private void Init(ControllerManager _controllerManager)
    {
        controllerManager = _controllerManager;
    }

    private void Start()
    {
        physicsObject = GetComponent<PhysicsObject>();
    }

    private void FixedUpdate()
    {
        float velocityX = controllerManager.HorizontalMove() * horizontalSpeed;

        if (physicsObject.IsGrounded() && controllerManager.JumpPressed())
        {
            physicsObject.Move(velocityX, jumpSpeed);
            jumpStartTime = Time.time;
        }
        else if (physicsObject.GetVelocityY() > 0f && controllerManager.JumpHolding())
        {
            float time = Time.time;
            if (time - jumpStartTime < maxJumpHoldTime)
                physicsObject.Move(velocityX, jumpSpeed);
        }
        else
            physicsObject.Move(velocityX);

        controllerManager.JumpPressedConsumed();
    }
}
