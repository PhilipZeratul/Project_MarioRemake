using UnityEngine;
using Zenject;


public class PlayerController : MonoBehaviour
{
    public float horizontalAcc = 5f;
    public float minHorizontalSpeed = 1f;
    public float maxHorizontalSpeed = 5f;
    public float jumpSpeed = 20f;
    public float maxJumpHoldTime = 0.5f;

    private PhysicsObject physicsObject;
    private ControllerManager controllerManager;
    private float jumpStartTime;
    private bool isFacingRight = true;
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

        if (physicsObject.GetVelocityX() * controllerManager.HorizontalInput() < 0)
            animator.SetBool(isTurningHash, true);
        else
            animator.SetBool(isTurningHash, false);
    }

    private void FixedUpdate()
    {
        HorizontalMove();

        Jump();
    }

    private void HorizontalMove()
    {
        velocityX = physicsObject.GetVelocityX();
        float accX;
        float horizontalInput = controllerManager.HorizontalInput();

        if (MyUtility.NearlyEqual(horizontalInput, 0f))
        {
            if (!MyUtility.NearlyEqual(velocityX, 0f))
                accX = -Mathf.Sign(velocityX) * horizontalAcc * 2f;
            else
                accX = 0f;
        }
        else 
        {
            if (Mathf.Sign(horizontalInput) * Mathf.Sign(velocityX) < 0)
            {
                accX = -Mathf.Sign(velocityX) * horizontalAcc * 3f;
            }
            else
            {
                if (Mathf.Abs(velocityX) > maxHorizontalSpeed)
                    accX = 0f;
                else
                {
                    accX = Mathf.Sign(velocityX) * horizontalAcc;

                    if (Mathf.Abs(velocityX) < minHorizontalSpeed)
                        velocityX = Mathf.Sign(velocityX) * minHorizontalSpeed;
                }
            }
        }

        if (velocityX * accX < 0)
            velocityX = velocityX > 0 ?
                Mathf.Max(0f, velocityX + accX * Time.fixedDeltaTime) :
                Mathf.Min(0f, velocityX + accX * Time.fixedDeltaTime);
        else
            velocityX += accX * Time.fixedDeltaTime;

        if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight))
            Flip();

        animator.SetFloat(runSpeedHash, velocityX);
    }

    private void Flip()
    {
        //
        Debug.LogFormat("Flip, velocityX = {0}", velocityX);


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
