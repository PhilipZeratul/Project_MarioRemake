using UnityEngine;


public class EnemyBase : MonoBehaviour, IInteractableObject
{
    public float moveSpeed = 2f;
    public Constants.EnemyMoveDireciton direction = Constants.EnemyMoveDireciton.Left;

    protected bool isActive = false;
    protected PhysicsObject physicsObject;
    protected Vector3 velocity;

    private Camera mainCamera;
    private Vector3 viewportPosition;


    private void Start()
    {
        mainCamera = Camera.main;

        physicsObject = GetComponent<PhysicsObject>();
        if (physicsObject != null)
            physicsObject.hitEvent += HitSelfReaction;
  
        velocity = Vector3.zero;
        switch (direction)
        {
            case Constants.EnemyMoveDireciton.Left:
                velocity.x = -moveSpeed;
                velocity.y = 0;
                break;
            case Constants.EnemyMoveDireciton.Right:
                velocity.x = moveSpeed;
                velocity.y = 0;
                break;
            case Constants.EnemyMoveDireciton.Up:
                velocity.x = 0;
                velocity.y = moveSpeed;
                break;
            case Constants.EnemyMoveDireciton.Down:
                velocity.x = 0;
                velocity.y = -moveSpeed;
                break;
            case Constants.EnemyMoveDireciton.Stop:
                velocity.x = 0;
                velocity.y = 0;
                break;
            default:
                Debug.LogErrorFormat("Enemy {0} move direction error!", gameObject);
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isActive)
        {
            viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPosition.x > 0f && viewportPosition.x <= 1f)
                isActive = true;
        }
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player != null)
        {
            if (from == Constants.HitDirection.Top)
            {
                player.BounceUponEnemy();
                Die();
            }
            else
            {
                // Kill Player
                Debug.LogFormat("Enemy: {0} Kill Player", gameObject.name);
                player.HitByEnemy();
            }
        }
    }

    private void HitSelfReaction(GameObject target, Constants.HitDirection dir)
    {
        if (target.tag == Constants.TagNames.Player)
            return;

        // Return when hit wall
        switch (direction)
        {
            case Constants.EnemyMoveDireciton.Left:
                if (dir == Constants.HitDirection.Right)
                {
                    direction = Constants.EnemyMoveDireciton.Right;
                    velocity.x = moveSpeed;
                }
                break;
            case Constants.EnemyMoveDireciton.Right:
                if (dir == Constants.HitDirection.Left)
                {
                    direction = Constants.EnemyMoveDireciton.Left;
                    velocity.x = -moveSpeed;
                }
                break;
            case Constants.EnemyMoveDireciton.Up:
                if (dir == Constants.HitDirection.Bottom)
                {
                    direction = Constants.EnemyMoveDireciton.Down;
                    velocity.y = -moveSpeed;
                }
                break;
            case Constants.EnemyMoveDireciton.Down:
                if (dir == Constants.HitDirection.Top)
                {
                    direction = Constants.EnemyMoveDireciton.Up;
                    velocity.y = moveSpeed;
                }
                break;
            default:
                Debug.LogErrorFormat("Enemy {0} move direction error!", gameObject);
                break;
        }
    }

    protected void Die()
    {
        Debug.LogFormat("Enemy: {0} Die()", gameObject.name);
    }
}
