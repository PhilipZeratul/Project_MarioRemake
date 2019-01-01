#define DEBUG_DRAW_RAY

using UnityEngine;
using System;


public class PhysicsObject : MonoBehaviour
{
    public Action<GameObject, Constants.HitDirection, bool> hitEvent;
    public float gravityScale = 1f;
    public float maxFallSpeed = -15f;

    public int numOfRaysX = 4;
    public int numOfRaysY = 3;
    public Collider2D collider2d;
    public float pushSpeed = 5f;

    private float velocityX;
    private float velocityY;
    private bool isGrounded;
    private float fixedDeltaTime;

    private Rect collisionRect;
    private readonly float rayEdgeMargin = 0.02f;
    private ContactFilter2D filter;
    private RaycastHit2D[] hits = new RaycastHit2D[4];
    private bool isXCollision, isYCollision;
    private float xAmendDistance, yAmendDistance;
    private float xRayDistance, yRayDistance;
    private GameObject xCollisionGO, yCollisionGO;

    private bool isMoveable = true;
    private bool hasGravity = true;
    private bool isInvinclible;


    private void Start()
    {    
        filter.useTriggers = false;
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        filter.useLayerMask = true;
    }

    private void FixedUpdate()
    {
        if (isMoveable)
        {
            fixedDeltaTime = Time.fixedDeltaTime;
            isGrounded = false;
            GetNewCollisionRect();

            isXCollision = false;
            xAmendDistance = 0f;
            xRayDistance = 0f;
            xCollisionGO = null;
            isYCollision = false;
            yAmendDistance = 0f;
            yRayDistance = 0f;
            yCollisionGO = null;

            // X
            float deltaX = velocityX * fixedDeltaTime;
            if (!MyUtility.NearlyEqual(deltaX, 0f))
                deltaX = XAxisCollision(deltaX);

            // Y
            float deltaY = 0;
            if (hasGravity)
                velocityY += gravityScale * Physics2D.gravity.y * fixedDeltaTime;

            velocityY = velocityY < maxFallSpeed ? maxFallSpeed : velocityY; // < because they are negative.
            deltaY = velocityY <= 0 ?
                YDownCollision(velocityY * fixedDeltaTime) :
                YUpCollision(velocityY * fixedDeltaTime, ref deltaX);

            ResolveCollision(ref deltaX, ref deltaY);
            transform.Translate(deltaX, deltaY, 0f);
        }
    }

    private float XAxisCollision(float deltaX)
    {    
        Vector2 rayStartPoint = new Vector2(collisionRect.center.x, collisionRect.yMin + rayEdgeMargin);
        Vector2 rayEndPoint = new Vector2(collisionRect.center.x, collisionRect.yMax - rayEdgeMargin);

        float distance = collisionRect.width / 2 + Mathf.Abs(deltaX);

        float minDeltaX = deltaX > 0 ? float.MaxValue : float.MinValue;
        GameObject hitTarget = null;
        for (int i = 0; i < numOfRaysX; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRaysX - 1);
            Vector2 origin = Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount);
            Vector2 dir = new Vector2(Mathf.Sign(deltaX), 0f);
           
            DebugDrawRay(origin, dir, distance, Color.yellow);

            int count = Physics2D.Raycast(origin, dir, filter, hits, distance);
            for (int j = 0; j < count; j++)
            {
                if (hits[j].transform.IsChildOf(transform))
                    continue;

                float x = (hits[j].distance - collisionRect.width / 2) * Mathf.Sign(deltaX);
                // Hit something
                if (Mathf.Abs(minDeltaX) > Mathf.Abs(x))
                {
                    minDeltaX = x;
                    hitTarget = hits[j].collider.gameObject;
                    xRayDistance = hits[j].distance;
                }
            }
        }

        if ((deltaX > 0 && minDeltaX < deltaX) ||
            (deltaX < 0 && minDeltaX > deltaX))
        {
            velocityX = 0f;
            isXCollision = true;
            xAmendDistance = minDeltaX - deltaX;
            xCollisionGO = hitTarget;
        }

        return deltaX;
    }

    private float YDownCollision(float deltaY)
    {
        Vector2 rayStartPoint = new Vector2(collisionRect.xMin + rayEdgeMargin, collisionRect.center.y);
        Vector2 rayEndPoint = new Vector2(collisionRect.xMax - rayEdgeMargin, collisionRect.center.y);

        float distance = collisionRect.height / 2 + Mathf.Abs(deltaY);

        float minDeltaY = float.MinValue;
        GameObject hitTarget = null;

        for (int i = 0; i < numOfRaysY; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRaysY - 1);
            Vector2 origin = Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount);
            Vector2 dir = new Vector2(0f, Mathf.Sign(deltaY));

            DebugDrawRay(origin, dir, distance, Color.white);

            int count = Physics2D.Raycast(origin, Vector2.down, filter, hits, distance);
            for (int j = 0; j < count; j++)
            {
                if (hits[j].transform.IsChildOf(transform))
                    continue;
                // Remove those hits that fires inside other colliders.
                if (MyUtility.NearlyEqual(hits[j].fraction, 0f))
                    continue;

                // Hit
                float y = -(hits[j].distance - collisionRect.height / 2);
                if (minDeltaY < y)
                {
                    minDeltaY = y;
                    hitTarget = hits[j].collider.gameObject;
                    yRayDistance = hits[j].distance;
                }
            }
        }

        if (minDeltaY > deltaY)
        {
            // If going downwards.
            isGrounded = true;
            velocityY = 0f;
            isYCollision = true;
            yAmendDistance = minDeltaY - deltaY;
            yCollisionGO = hitTarget;
        }
        return deltaY;
    }

    // YAxisCollision can change deltaX when jumping up and pushed sideways to be able to jump on platform.
    private float YUpCollision(float deltaY, ref float deltaX)
    {
        Vector2 rayStartPoint = new Vector2(collisionRect.xMin + rayEdgeMargin, collisionRect.center.y);
        Vector2 rayEndPoint = new Vector2(collisionRect.xMax - rayEdgeMargin, collisionRect.center.y);

        float distance = collisionRect.height / 2 + deltaY;

        float minDeltaY = float.MaxValue;
        GameObject hitTarget = null;

        int hitRayNo = 0;
        bool isMiddleHitGround = false;

        for (int i = 0; i < numOfRaysY; i++)
        {
            float lerpAmount = (float)i / ((float)numOfRaysY - 1);
            Vector2 origin = Vector2.Lerp(rayStartPoint, rayEndPoint, lerpAmount);
            Vector2 dir = new Vector2(0f, Mathf.Sign(deltaY));

            DebugDrawRay(origin, dir, distance, Color.white);

            int count = Physics2D.Raycast(origin, Vector2.up, filter, hits, distance);
            for (int j = 0; j < count; j++)
            {
                if (hits[j].transform.IsChildOf(transform))
                    continue;

                // Hit
                //~TODO: Fix this for general use.
                // The middle ray shotting up hitted ground
                if (i == 1 && hits[j].collider.gameObject.layer == LayerMask.NameToLayer(Constants.LayerNames.Ground))
                {
                    isMiddleHitGround = true;
                }

                float y = hits[j].distance - collisionRect.height / 2;
                if (minDeltaY > y)
                {
                    minDeltaY = y;
                    hitTarget = hits[j].collider.gameObject;
                    hitRayNo = i;
                    yRayDistance = hits[j].distance;
                }
            }
        }

        if (minDeltaY < deltaY && hitTarget != null)
        {
            if (!isMiddleHitGround && hitTarget.layer == LayerMask.NameToLayer(Constants.LayerNames.Ground))
            {
                if (hitRayNo == 0)
                {
                    deltaX += pushSpeed * Time.fixedDeltaTime;
                }
                else if (hitRayNo == 2)
                {
                    deltaX -= pushSpeed * Time.fixedDeltaTime;
                }
            }
            else
            {
                velocityY = 0f;
                isYCollision = true;
                yAmendDistance = minDeltaY - deltaY;
                yCollisionGO = hitTarget;
            }

        }
        return deltaY;
    }

    private void ResolveCollision(ref float deltaX, ref float deltaY)
    {
        if (isXCollision && isYCollision && xCollisionGO == yCollisionGO && xCollisionGO != null)
        {
            if (Math.Abs(xRayDistance) <= Math.Abs(yRayDistance))
            {
                xAmendDistance = 0f;
                isXCollision = false;
            }
            else
            {
                yAmendDistance = 0f;
                isYCollision = false;
            }
        }

        if (isXCollision)
        {
            deltaX += xAmendDistance;
            Hit(xCollisionGO, deltaX > 0 ? Constants.HitDirection.Left : Constants.HitDirection.Right, Math.Abs(xRayDistance) < (collisionRect.width / 2 - 0.05f));
        }
        if (isYCollision)
        {
            deltaY += yAmendDistance;
            Hit(yCollisionGO, deltaY > 0 ? Constants.HitDirection.Bottom : Constants.HitDirection.Top, Math.Abs(yRayDistance) < (collisionRect.height / 2 - 0.05f));
        }
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

    private void Hit(GameObject target, Constants.HitDirection dir, bool isHitInside)
    {
        // Hit self reaction
        if (hitEvent != null)
            hitEvent(target, dir, isHitInside);

        // Hit notify the other
        IInteractableObject interactableObj = target.GetComponentRecursiveUp<IInteractableObject>();
        if (interactableObj != null)
        {
            //Debug.LogFormat("Object: {2}, Hit: {0}, From: {1}", target.name, dir, gameObject.name);
            interactableObj.IsHit(gameObject, dir);
        }
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

    public void SetMoveable(bool value)
    {
        isMoveable = value;
    }

    public void SetGravity(bool value)
    {
        hasGravity = value;
    }

    public void SetInvincible(bool value)
    {
        isInvinclible = value;
    }

    public bool IsInvincible()
    {
        return isInvinclible;
    }

    [System.Diagnostics.Conditional("DEBUG_DRAW_RAY")]
    private void DebugDrawRay(Vector2 origin, Vector2 dir, float distance, Color color)
    {
        Debug.DrawLine(origin, origin + dir * distance, color);
    }
}
