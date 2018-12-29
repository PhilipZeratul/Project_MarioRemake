public class GroundEnemy : EnemyBase
{
    private void FixedUpdate()
    {
        if (physicsObject != null && isActive && !MyUtility.NearlyEqual(moveSpeed, 0f))
        {
            physicsObject.Move(velocity.x);
        }
    }
}
