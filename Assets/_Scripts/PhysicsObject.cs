using UnityEngine;


public class PhysicsObject : MonoBehaviour
{
    public float horizontalSpeed = 8f;
    public float jumpSpeed = 8f;
    public float gravityScale = 1f;

    public int numOfRays = 3;
    public Collider2D collider2d;

    private Vector2 velocity;
    private bool isGrounded;
    private float fixedDeltaTime;
    private Rect collisionRect;
    private readonly float rayMargin = 0.1f;
    private readonly float distanceMargin = 0.1f;
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

        velocity = new Vector2(0f, velocity.y + gravityScale * Physics2D.gravity.y * fixedDeltaTime);

        float deltaY = YAxisCollision(velocity.y * fixedDeltaTime, 1f);

        Move(0f, deltaY);
    }

    private void Move(float deltaX, float deltaY)
    {
        transform.Translate(deltaX, deltaY, 0f);
    }

    private float YAxisCollision(float deltaY, float dirX)
    {
        GetNewCollisionRect();

        Vector2 rayStartPoint = new Vector2(collisionRect.xMin + rayMargin, collisionRect.center.y);
        Vector2 rayEndPoint = new Vector2(collisionRect.xMax - rayMargin, collisionRect.center.y);

        float distance = collisionRect.height / 2 + Mathf.Abs(deltaY);

        float minDeltaY = float.MaxValue;
        for (int i = 0; i < numOfRays; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRays - 1);
            Vector2 origin = MyUtility.NearlyEqual(dirX, -1f) ?
                                      Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount) :
                                      Vector2.Lerp(rayEndPoint, rayStartPoint, lerpAmount);

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
            velocity = new Vector2(velocity.x, 0f);
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
}
