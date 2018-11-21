using UnityEngine;


public class Flag : MonoBehaviour, IInteractableObject
{
    private readonly int finishHash = Animator.StringToHash("Finish");
    private Animator animator;


    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player == null)
            return;

        animator.SetTrigger(finishHash);
        player.LevelFinished();
    }
}
