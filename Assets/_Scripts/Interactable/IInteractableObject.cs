using UnityEngine;


public interface IInteractableObject
{
    void IsHit(GameObject source, Constants.HitDirection from);
}
