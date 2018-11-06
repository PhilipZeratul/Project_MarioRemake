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
    private bool isFacingRight = true;

    private Animator animator;
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");


    [Inject]
    private void Init(ControllerManager _controllerManager)
    {
        controllerManager = _controllerManager;
    }

    private void Start()
    {
        physicsObject = GetComponent<PhysicsObject>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        animator.SetBool(isGroundedHash, physicsObject.IsGrounded());
        animator.SetBool(isRunningHash, !MyUtility.NearlyEqual(physicsObject.GetVelocityX(), 0f));
    }

    private void FixedUpdate()
    {
        float velocityX = controllerManager.HorizontalMove() * horizontalSpeed;

        if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight))
            Flip();

        if (physicsObject.IsGrounded() && controllerManager.JumpPressed())
        {        
            Jump(velocityX);
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

    private void Jump(float velocityX)
    {
        physicsObject.Move(velocityX, jumpSpeed);
        jumpStartTime = Time.time;

        animator.SetTrigger(jumpHash);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
