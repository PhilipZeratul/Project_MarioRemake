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
    private float minCollisionNormal = 0.9f;
    private float collisionShell = 0.02f;
    private float skinSpace = 0.005f;


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

        CollisionCheck();

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

    private void CollisionCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = false;

        float newPositionX = rb2d.position.x;
        float newPositionY = rb2d.position.y;

        //float distance = velocity.magnitude * fixedDeltaTime + collisionShell;

        float distanceX = velocity.x * fixedDeltaTime + collisionShell;
        float distanceY = velocity.y * fixedDeltaTime + collisionShell;

        int countX = rb2d.Cast(new Vector2(velocity.x, 0f), contactFilter2D, raycastHit2Ds, distanceX);
        Debug.LogFormat("CollisionCheck(), countX = {0}", countX);

        for (int i = 0; i < countX; i++)
        {
            Debug.LogFormat("Horizontal Collision, distance = {0}, normal = {1}, point = {2}", raycastHit2Ds[i].distance.ToString("F4"), raycastHit2Ds[i].normal.ToString("F4"), raycastHit2Ds[i].point.ToString("F4"));
            // Horizontal Collision.
            if (-raycastHit2Ds[i].normal.x * Mathf.Sign(velocity.x) > minCollisionNormal)
            {
                velocity = new Vector2(0f, velocity.y);
                newPositionX = Mathf.Round(raycastHit2Ds[i].point.x) + Mathf.Sign(raycastHit2Ds[i].normal.x) * (0.5f + skinSpace);
            }
        }


        int countY = rb2d.Cast(new Vector2(0f, velocity.y), contactFilter2D, raycastHit2Ds, distanceY);
        Debug.LogFormat("CollisionCheck(), countY = {0}", countY);

        for (int i = 0; i < countY; i++)
        {
            // Collide with ground.
            if (whatIsGround == (whatIsGround | (1 << raycastHit2Ds[i].collider.gameObject.layer)))
            {
                // Vertical Down collision.
                if (velocity.y < 0 && raycastHit2Ds[i].normal.y > minCollisionNormal)
                {
                    Debug.Log("Down Collision.");
                    isGrounded = true;
                    velocity = new Vector2(velocity.x, 0f);
                    if (!wasGrounded)
                    {
                        // Round the final y position, so that character wouldn't stuck in ground.
                        newPositionY = Mathf.Round(raycastHit2Ds[i].point.y) + skinSpace;
                        OnLanded();
                    }
                }
                // Vertical Up collision.
                else if (velocity.y > 0 && -raycastHit2Ds[i].normal.y > minCollisionNormal)
                {
                    Debug.Log("Up Collision.");
                    velocity = new Vector2(velocity.x, 0f);
                    newPositionY = rb2d.position.y - raycastHit2Ds[i].distance * raycastHit2Ds[i].normal.y;
                }
            }
        }

        rb2d.position = new Vector2(newPositionX, newPositionY);
    }

    private void Move()
    {
        Debug.LogFormat("velocity = {0}", velocity);
        rb2d.velocity = velocity;

        //Debug.LogFormat("velocity = {0}", velocity.ToString("F4"));
        //rb2d.MovePosition(rb2d.position += velocity * fixedDeltaTime);
    }

    protected virtual void OnLanded()
    {
    }
}
