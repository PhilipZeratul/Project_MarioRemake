using UnityEngine;
using Zenject;


public class PlayerController : MonoBehaviour
{
    public float horizontalAcc = 5f;
    public float sprintAdder = 1.5f;
    public float minHorizontalSpeed = 1f;
    public float maxHorizontalSpeed = 5f;
    public float jumpSpeed = 20f;
    public float maxJumpHoldTime = 0.5f;
    public float animationPlaybackMultiplier = 0.2f;
    public float minAnimationPlaybackMultiplier = 0.5f;

    private PhysicsObject physicsObject;
    private ControllerManager controllerManager;
    private float jumpStartTime;
    private bool isFacingRight = true;
    private bool isTurning;
    private bool isSprinting;
    private float velocityX;

    private Animator animator;
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int isTurningHash = Animator.StringToHash("IsTurning");
    private readonly int runSpeedHash = Animator.StringToHash("RunSpeed");


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
        animator.SetBool(isTurningHash, isTurning);
    }

    private void FixedUpdate()
    {
        HorizontalMove();

        Jump();
    }

    private void HorizontalMove()
    {
        velocityX = physicsObject.GetVelocityX();
        float accX = 0f;
        float horizontalInput = controllerManager.HorizontalInput();

        float maxSpeed = controllerManager.SprintHolding() ?
                             maxHorizontalSpeed + sprintAdder : maxHorizontalSpeed;
        float minSpeed = controllerManager.SprintHolding() ?
                             minHorizontalSpeed + sprintAdder : minHorizontalSpeed;                                          

        if (isTurning)
        {
            if (MyUtility.NearlyEqual(velocityX, 0f))
                isTurning = false;
            else
                accX = controllerManager.SprintHolding() ?
                           -Mathf.Sign(velocityX) * horizontalAcc * 4f :
                           -Mathf.Sign(velocityX) * horizontalAcc * 3f;
        }
        else
        {
            // No Input
            if (MyUtility.NearlyEqual(horizontalInput, 0f))
            {
                if (!MyUtility.NearlyEqual(velocityX, 0f))
                    accX = -Mathf.Sign(velocityX) * horizontalAcc * 2f;
            }
            // Have Input
            else
            {
                // Issue Turning
                if (horizontalInput * velocityX < -0.1f)
                {
                    isTurning = true;
                }
                else if (Mathf.Abs(velocityX) < maxSpeed)                         
                {
                    accX = controllerManager.SprintHolding() ?
                               Mathf.Sign(horizontalInput) * horizontalAcc * 2 :
                               Mathf.Sign(horizontalInput) * horizontalAcc;

                    if (Mathf.Abs(velocityX) < minSpeed)
                        velocityX = Mathf.Sign(accX) * minSpeed;
                }
            }
        }

        if (velocityX * accX < 0)
            velocityX = velocityX > 0 ?
                Mathf.Max(0f, velocityX + accX * Time.fixedDeltaTime) :
                Mathf.Min(0f, velocityX + accX * Time.fixedDeltaTime);
        else
            velocityX += accX * Time.fixedDeltaTime;

        if (Mathf.Abs(velocityX) > maxSpeed)
            velocityX = Mathf.Sign(velocityX) * maxSpeed;

        if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight))
            Flip();

        if (!MyUtility.NearlyEqual(velocityX, 0f))
            animator.SetFloat(runSpeedHash, Mathf.Max(Mathf.Abs(animationPlaybackMultiplier * velocityX), minAnimationPlaybackMultiplier));            
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void Jump()
    {
        if (physicsObject.IsGrounded() && controllerManager.JumpPressed())
        {
            physicsObject.Move(velocityX, jumpSpeed);
            jumpStartTime = Time.time;

            animator.SetTrigger(jumpHash);
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
