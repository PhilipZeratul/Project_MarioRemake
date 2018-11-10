using UnityEngine;


public class PowerupMushroom : MonoBehaviour, IInteractableObject
{
    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeToBig();
        }
    }
}
