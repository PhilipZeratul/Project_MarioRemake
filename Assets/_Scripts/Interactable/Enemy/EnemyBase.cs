using UnityEngine;
using Zenject;


public class EnemyBase : MonoBehaviour, IInteractableObject
{
    public float moveSpeed = 2f;
    public Constants.EnemyMoveDireciton direction = Constants.EnemyMoveDireciton.Left;

    protected PhysicsObject physicsObject;
    protected Vector3 velocity;
    protected bool isActive = false;
    protected Animator animator;
    protected GameManager gameManager;


    [Inject]
    private void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    private void Start()
    {    
        physicsObject = GetComponent<PhysicsObject>();
        if (physicsObject != null)
            physicsObject.hitEvent += HitSelfReaction;

        animator = GetComponent<Animator>();

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

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player != null)
        {
            if (from == Constants.HitDirection.Top)
            {
                player.BounceUponEnemy();
                TreadDie();
            }
            else
            {
                // Kill Player
                Debug.LogFormat("Enemy: {0} Kill Player", gameObject.name);
                player.HitByEnemy();
            }
        }
    }

    private void HitSelfReaction(GameObject target, Constants.HitDirection dir, bool isHitInside)
    {
        if (target.tag == Constants.TagNames.Player)
        {
            IsHit(target.transform.root.gameObject, dir);
            return;
        }


        if (isHitInside && dir == Constants.HitDirection.Bottom && target.tag == Constants.TagNames.Brick)
        {
            HitDie();
            return;
        }

        // Return when hit wall
        switch (direction)
        {
            case Constants.EnemyMoveDireciton.Left:
                if (dir == Constants.HitDirection.Right)
                {
                    direction = Constants.EnemyMoveDireciton.Right;
                    velocity.x = moveSpeed;
                    transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
                }
                break;
            case Constants.EnemyMoveDireciton.Right:
                if (dir == Constants.HitDirection.Left)
                {
                    direction = Constants.EnemyMoveDireciton.Left;
                    velocity.x = -moveSpeed;
                    transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
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

    protected virtual void TreadDie(){}

    protected virtual void HitDie() { }

    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;
    }
}
