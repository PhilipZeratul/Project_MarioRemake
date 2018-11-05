using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    public float gravityScale = 1f;

    public int numOfRays = 3;
    public Collider2D collider2d;

    private float velocityX;
    private float velocityY;
    private bool isGrounded;
    private float fixedDeltaTime;
    private Rect collisionRect;
    private readonly float rayEdgeMargin = 0.1f;
    private ContactFilter2D filter;
    private RaycastHit2D[] hits = new RaycastHit2D[4];


    private void Start()
    {    
        filter.useTriggers = false;
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        filter.useLayerMask = true;
    }

    private void FixedUpdate()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
        isGrounded = false;        

        velocityY += gravityScale * Physics2D.gravity.y * fixedDeltaTime;

        float deltaY = YAxisCollision(velocityY * fixedDeltaTime);
        float deltaX = XAxisCollision(velocityX * fixedDeltaTime);

        transform.Translate(deltaX, deltaY, 0f);
    }

    public void Move(float inputVelocityX)
    {
        velocityX = inputVelocityX;
    }

    public void Move(float inputVelocityX, float inputVelocityY)
    {
        velocityX = inputVelocityX;
        velocityY = inputVelocityY;
    }

    private float XAxisCollision(float deltaX)
    {
        return deltaX;

    }

    private float YAxisCollision(float deltaY)
    {
        GetNewCollisionRect();

        Vector2 rayStartPoint = new Vector2(collisionRect.xMin + rayEdgeMargin, collisionRect.center.y);
        Vector2 rayEndPoint = new Vector2(collisionRect.xMax - rayEdgeMargin, collisionRect.center.y);

        float distance = collisionRect.height / 2 + Mathf.Abs(deltaY);

        float minDeltaY = float.MaxValue;
        for (int i = 0; i < numOfRays; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRays - 1);
            Vector2 origin = Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount);

            ///
            Vector2 end = origin + new Vector2(0f, Mathf.Sign(deltaY)) * distance;
            Debug.DrawLine(origin, end, Color.white);
            ///

            int count = Physics2D.Raycast(origin, new Vector2(0f, Mathf.Sign(deltaY)), filter, hits, distance);
            for (int j = 0; j < count; j++)
            {
                float y = (hits[j].distance - collisionRect.height / 2) * Mathf.Sign(deltaY);
                if (Mathf.Abs(minDeltaY) > Mathf.Abs(y))
                    minDeltaY = y;
            }
        }

        if (Mathf.Abs(minDeltaY) < Mathf.Abs(deltaY))
        {
            if (Mathf.Sign(deltaY) < 0)
            {
                isGrounded = true;
            }

            velocityY = 0f;
            deltaY = minDeltaY;
        }
        return deltaY;
    }

    private void GetNewCollisionRect()
    {
        collisionRect = new Rect(
            collider2d.bounds.min.x,
            collider2d.bounds.min.y,
            collider2d.bounds.size.x,
            collider2d.bounds.size.y
        );
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
