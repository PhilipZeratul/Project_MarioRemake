using UnityEngine;


public class Brick : MonoBehaviour, IInteractableObject
{
    public GameObject brickBreakParticleGO;


    public void IsHit(GameObject source, Constants.HitDirection dir)
    {
        if (dir == Constants.HitDirection.Bottom)
        {

            Instantiate(brickBreakParticleGO, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
