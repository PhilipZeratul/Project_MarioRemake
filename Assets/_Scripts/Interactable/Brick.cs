using UnityEngine;


public class Brick : MonoBehaviour, IInteractableObject
{
    public GameObject brickBreakParticleGO;


    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        if (from == Constants.HitDirection.Bottom)
        {

            Instantiate(brickBreakParticleGO, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
