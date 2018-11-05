using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    public float gravityScale = 1f;

    public int numOfRaysX = 4;
    public int numOfRaysY = 3;
    public Collider2D collider2d;

    private float velocityX;
    private float velocityY;
    private bool isGrounded;
    private float fixedDeltaTime;
    private Rect collisionRect;
    private readonly float rayEdgeMargin = 0.02f;
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
        GetNewCollisionRect();

        float deltaX = velocityX * fixedDeltaTime;
        if (!MyUtility.NearlyEqual(deltaX, 0f))
            deltaX = XAxisCollision(deltaX);

        velocityY += gravityScale * Physics2D.gravity.y * fixedDeltaTime;
        float deltaY = YAxisCollision(velocityY * fixedDeltaTime);

        transform.Translate(deltaX, deltaY, 0f);
    }

    private float XAxisCollision(float deltaX)
    {
        Vector2 rayStartPoint = new Vector2(collisionRect.center.x, collisionRect.yMin + rayEdgeMargin);
        Vector2 rayEndPoint = new Vector2(collisionRect.center.x, collisionRect.yMax - rayEdgeMargin);

        float distance = collisionRect.width / 2 + Mathf.Abs(deltaX);

        float minDeltaX = deltaX > 0 ? float.MaxValue : float.MinValue;
        for (int i = 0; i < numOfRaysX; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRaysX - 1);
            Vector2 origin = Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount);

            ///
            Vector2 end = origin + new Vector2(Mathf.Sign(deltaX), 0f) * distance;
            Debug.DrawLine(origin, end, Color.yellow);
            ///

            int count = Physics2D.Raycast(origin, new Vector2(Mathf.Sign(deltaX), 0f), filter, hits, distance);
            for (int j = 0; j < count; j++)
            {
                float x = (hits[j].distance - collisionRect.width / 2) * Mathf.Sign(deltaX);
                if (Mathf.Abs(minDeltaX) > Mathf.Abs(x))
                    minDeltaX = x;
            }
        }

        if ((deltaX > 0 && minDeltaX < deltaX) ||
            (deltaX < 0 && minDeltaX > deltaX))
        {
            velocityX = 0f;
            deltaX = minDeltaX;
        }
        return deltaX;
    }

    private float YAxisCollision(float deltaY)
    {    
        Vector2 rayStartPoint = new Vector2(collisionRect.xMin + rayEdgeMargin, collisionRect.center.y);
        Vector2 rayEndPoint = new Vector2(collisionRect.xMax - rayEdgeMargin, collisionRect.center.y);

        float distance = collisionRect.height / 2 + Mathf.Abs(deltaY);

        float minDeltaY = deltaY > 0 ? float.MaxValue : float.MinValue;
        for (int i = 0; i < numOfRaysY; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRaysY - 1);
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

        if ((deltaY > 0 && minDeltaY < deltaY) ||
            (deltaY < 0 && minDeltaY > deltaY))
        {
            // If going downwards.
            isGrounded |= Mathf.Sign(deltaY) < 0;

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

    public void Move(float inputVelocityX)
    {
        velocityX = inputVelocityX;
    }

    public void Move(float inputVelocityX, float inputVelocityY)
    {
        velocityX = inputVelocityX;
        velocityY = inputVelocityY;
    }

    public float GetVelocityX()
    {
        return velocityX;
    }

    public float GetVelocityY()
    {
        return velocityY;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
