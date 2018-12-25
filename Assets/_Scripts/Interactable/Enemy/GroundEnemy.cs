public class GroundEnemy : EnemyBase
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isActive && physicsObject != null && !MyUtility.NearlyEqual(moveSpeed, 0f))
        {
            physicsObject.Move(velocity.x);
        }
    }
}
