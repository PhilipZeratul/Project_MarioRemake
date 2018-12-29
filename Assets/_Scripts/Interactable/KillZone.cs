using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class KillZone : MonoBehaviour, IInteractableObject
{
    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController playerController = source.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.Die();
        }
        else
        {
            Destroy(source);
        }
    }
}
