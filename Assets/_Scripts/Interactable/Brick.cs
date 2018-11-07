using UnityEngine;


public class Brick : MonoBehaviour, IInteractableObject
{
    public void IsHit(GameObject source, Constants.HitDirection dir)
    {
        if (dir == Constants.HitDirection.Bottom)
            Destroy(gameObject);
    }
}
