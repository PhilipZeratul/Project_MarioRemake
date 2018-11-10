using UnityEngine;


public class CoinBrick : MonoBehaviour, IInteractableObject
{
    public int coinNum = 1;
    public GameObject coin;
    private GameObject spriteGO;
    private Animator animator;

    private readonly int hitHash = Animator.StringToHash("Hit");
    private readonly int deadHash = Animator.StringToHash("Dead");


    private void Start()
    {
        spriteGO = GetComponentInChildren<SpriteRenderer>().gameObject;
        animator = GetComponent<Animator>();
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        if (from == Constants.HitDirection.Bottom)
        {
            if (coinNum < 1)
                return;

            coinNum--;
            Instantiate(coin, transform.position, transform.rotation);
            animator.SetTrigger(hitHash);
            if (coinNum < 1)
                animator.SetBool(deadHash, true);
        }
    }
}
