using UnityEngine;


public class Brick : MonoBehaviour, IInteractableObject
{
    public GameObject brickBreakParticleGO;

    private Animator animator;
    private int hitHash = Animator.StringToHash("Hit");


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player == null)
            return;
            
        if (from == Constants.HitDirection.Bottom)
        {
            if (player.IsBigMario())
            {            
                Instantiate(brickBreakParticleGO, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                animator.SetTrigger(hitHash);
            }
        }
    }
}
