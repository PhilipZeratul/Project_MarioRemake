using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    public float horizontalSpeed = 8f;
    public float jumpSpeed = 8f;
    public float gravityScale = 1f;
    public LayerMask whatIsGround;

    protected bool isGrounded = false;
    protected bool wasGrounded = false;
    protected float fixedDeltaTime;

    private Rigidbody2D rb2d;
    private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[16];
    private ContactFilter2D contactFilter2D;
    private Vector2 velocity;


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        contactFilter2D.useTriggers = false;
        contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter2D.useLayerMask = true;

        if (whatIsGround == 0)
            Debug.LogWarning("Please set whatIsGround property.");
    }

    private void FixedUpdate()
    {
        fixedDeltaTime = Time.deltaTime;

        float horizontalV = CalcHorizontalVelocity();
        float verticalV = CalcVerticalVelocity() + gravityScale * Physics2D.gravity.y * fixedDeltaTime;

        velocity = new Vector2(horizontalV, velocity.y + verticalV);

        GroundCheck();

        Move();
    }

    protected virtual float CalcHorizontalVelocity()
    {
        return 0f;
    }

    protected virtual float CalcVerticalVelocity()
    {
        return 0f;
    }

    private void GroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = false;

        if (velocity.y > 0)
            return;

        float groundCheckDistance = -velocity.y * fixedDeltaTime + 0.01f;
        int count = rb2d.Cast(Vector2.down, contactFilter2D, raycastHit2Ds, groundCheckDistance);

        for (int i = 0; i < count; i++)
        {
            if (whatIsGround == (whatIsGround | (1 << raycastHit2Ds[i].collider.gameObject.layer)))
            {
                isGrounded = true;
                velocity = new Vector2(velocity.x, 0f);
                if (!wasGrounded)
                {                    
                    // Round the final y position, so that character wouldn't stuck in ground.
                    rb2d.position = new Vector2(rb2d.position.x, Mathf.Round(raycastHit2Ds[i].point.y));
                    OnLanded();
                }
                break;
            }
        }
    }

    private void Move()
    {
        rb2d.velocity = velocity;
    }

    protected virtual void OnLanded()
    {
        Debug.Log("OnLanded()");
    }
}
