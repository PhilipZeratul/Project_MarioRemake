using UnityEngine;


public class EnemyBase : MonoBehaviour, IInteractableObject
{
    public Transform[] wayPoints;
    public float moveSpeed = 2f;

    private int currentWayPoint;
    private PhysicsObject physicsObject;


    private void Start()
    {
        if (wayPoints.Length > 0)
            physicsObject = GetComponent<PhysicsObject>();
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;

        if (wayPoints.Length > 0)
        {
            Vector3 direction = (wayPoints[currentWayPoint].position - transform.position).normalized;
            physicsObject.Move(direction.x * moveSpeed, direction.y * moveSpeed);

            float distance = Vector3.Distance(transform.position, wayPoints[currentWayPoint].position);
            if (distance < moveSpeed * fixedDeltaTime)
                currentWayPoint = (currentWayPoint + 1) % wayPoints.Length;

            Debug.LogFormat("currentWayPoint = {0}, direction = {3}, distance = {1}, threshold = {2}", currentWayPoint, distance, moveSpeed * fixedDeltaTime, direction);
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
        else
        {
            //~TODO: Return when hit wall.
            //currentWayPoint--;
            //if (currentWayPoint < 0)
                //currentWayPoint = wayPoints.Length - 1;
        }
    }

    protected void Die()
    {
        Debug.LogFormat("Enemy: {0} Die()", gameObject.name);
    }
}
