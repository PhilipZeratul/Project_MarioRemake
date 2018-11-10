﻿using UnityEngine;
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
    public Collider2D smallMarioCollider;
    public Collider2D bigMarioCollider;

    private PhysicsObject physicsObject;
    private ControllerManager controllerManager;
    private float jumpStartTime;
    private bool isFacingRight = true;
    private bool isTurning;
    private float velocityX;
    private bool isBigMario;

    private Animator animator;
    private readonly int jumpHash = Animator.StringToHash("TriggerJump");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int isTurningHash = Animator.StringToHash("IsTurning");
    private readonly int runSpeedHash = Animator.StringToHash("RunSpeed");
    private readonly int isBigHash = Animator.StringToHash("IsBig");
    private readonly int triggerBigHash = Animator.StringToHash("TriggerToBig");
    private readonly int triggerSmallHash = Animator.StringToHash("TriggerToSmall");


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

    public void ChangeToBig()
    {
        if (isBigMario)
            return;

        isBigMario = true;
        animator.SetTrigger(triggerBigHash);
        animator.SetBool(isBigHash, true);
        bigMarioCollider.enabled = true;
        smallMarioCollider.enabled = false;
        physicsObject.collider2d = bigMarioCollider;
    }

    public void ChangeToSmall()
    {
        if (!isBigMario)
            return;

        isBigMario = false;
        animator.SetTrigger(triggerSmallHash);
        animator.SetBool(isBigHash, false);
        bigMarioCollider.enabled = false;
        smallMarioCollider.enabled = true;
        physicsObject.collider2d = smallMarioCollider;
    }

    public void HitByEnemy()
    {
        if (isBigMario)
            ChangeToSmall();
        else
            Die();
    }

    private void Die()
    {
        Debug.Log("Mario Die()");
    }
}
