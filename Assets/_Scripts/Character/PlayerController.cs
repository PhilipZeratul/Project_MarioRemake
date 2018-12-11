using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using Zenject;


public class PlayerController : MonoBehaviour
{
    public float horizontalAcc = 5f;
    public float sprintAdder = 1.5f;
    public float minHorizontalSpeed = 1f;
    public float maxHorizontalSpeed = 5f;
    public float jumpSpeed = 20f;
    public float bounceSpeed = 15f;
    public float maxJumpHoldTime = 0.5f;
    public float animationPlaybackMultiplier = 0.2f;
    public float minAnimationPlaybackMultiplier = 0.5f;
    public Collider2D smallMarioCollider;
    public Collider2D bigMarioCollider;
    public float unMoveableDuration = 1f;
    public float invincibleDuration = 1.5f;
    public float climbFlagSpeed = 1f;

    private PhysicsObject physicsObject;
    private ControllerManager controllerManager;
    private float jumpStartTime;
    private bool isFacingRight = true;
    private bool isTurning;
    private bool isCrouching;
    private float velocityX;
    private bool isBigMario;
    private WaitForSeconds waitForMoveable;

    private Animator animator;
    private PlayableDirector playableDirector;
    private SpriteRenderer marioSprite;
    private LevelManager levelManager;

    private readonly int jumpHash = Animator.StringToHash("TriggerJump");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int isTurningHash = Animator.StringToHash("IsTurning");
    private readonly int isCrouchingHash = Animator.StringToHash("IsCrouching");
    private readonly int runSpeedHash = Animator.StringToHash("RunSpeed");
    private readonly int isBigHash = Animator.StringToHash("IsBig");
    private readonly int triggerBigHash = Animator.StringToHash("TriggerToBig");
    private readonly int triggerSmallHash = Animator.StringToHash("TriggerToSmall");
    private readonly int isClimbingFlag = Animator.StringToHash("IsClimbingFlag");


    [Inject]
    private void Init(ControllerManager _controllerManager)
    {
        controllerManager = _controllerManager;
    }

    private void Start()
    {
        physicsObject = GetComponent<PhysicsObject>();
        animator = GetComponentInChildren<Animator>();
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.stopped += OnFinishCutSceneFinished;
        marioSprite = GetComponentInChildren<SpriteRenderer>();

        waitForMoveable = new WaitForSeconds(unMoveableDuration);
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

        // Crouching
        if (controllerManager.DownHolding() && isBigMario && physicsObject.IsGrounded())
        {
            isCrouching = true;
            if (!MyUtility.NearlyEqual(velocityX, 0f))
                accX = -Mathf.Sign(velocityX) * horizontalAcc * 4f;

        }
        else
        {
            isCrouching = false;
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

        animator.SetBool(isCrouchingHash, isCrouching);

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

        StartCoroutine(WaitForMoveable());
        StartCoroutine(WaitForInvincible(invincibleDuration));
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

        StartCoroutine(WaitForMoveable());
        StartCoroutine(WaitForInvincible(invincibleDuration));
    }

    public void HitByEnemy()
    {
        if (physicsObject.IsInvincible())
            return;

        if (isBigMario)
            ChangeToSmall();
        else
            Die();
    }

    public void BounceUponEnemy()
    {
        physicsObject.Move(physicsObject.GetVelocityX(), bounceSpeed);
    }

    private void Die()
    {
        Debug.Log("Mario Die()");
    }

    private IEnumerator WaitForMoveable()
    {
        physicsObject.SetMoveable(false);
        yield return waitForMoveable;
        physicsObject.SetMoveable(true);
    }

    private IEnumerator WaitForInvincible(float seconds)
    {
        physicsObject.SetInvincible(true);
        yield return new WaitForSeconds(seconds);
        physicsObject.SetInvincible(false);
    }

    public bool IsBigMario()
    {
        return isBigMario;
    }

    public void LevelStart(LevelManager manager)
    {
        Debug.Log("LevelStart()");

        levelManager = manager;
        marioSprite.enabled = true;
        controllerManager.SetControllable(true);
        physicsObject.SetGravity(true);
    }

    public void LevelFinish()
    {
        Debug.Log("LevelFinish()");

        controllerManager.SetControllable(false);
        physicsObject.SetGravity(false);
        animator.SetBool(isClimbingFlag, true);

        StartCoroutine(ClimbFlag());
    }

    private IEnumerator ClimbFlag()
    {
        while (!physicsObject.IsGrounded())
        {
            physicsObject.Move(0f, -climbFlagSpeed);
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool(isClimbingFlag, false);

        // Play End Cut Scene
        marioSprite.enabled = false;
        playableDirector.Play();
    }

    private void OnFinishCutSceneFinished(PlayableDirector director)
    {
        levelManager.ToNextLevel();
    }
}
