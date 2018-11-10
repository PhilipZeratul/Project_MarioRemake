using UnityEngine;


public class EnemyBase : MonoBehaviour, IInteractableObject
{
    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player == null)
            return;        

        if (from == Constants.HitDirection.Top)
        {
            Die();
        }
        else
        {
            // Kill Player
            Debug.LogFormat("Enemy: {0} Kill Player", gameObject.name);
            player.HitByEnemy();
        }

    }

    protected void Die()
    {
        Debug.LogFormat("Enemy: {0} Die()", gameObject.name);
    }
}
