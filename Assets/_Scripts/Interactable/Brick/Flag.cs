using UnityEngine;


public class Flag : MonoBehaviour, IInteractableObject
{
    private Animator animator;
    private readonly int isFinishedHash = Animator.StringToHash("IsFinished");


    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player == null)
            return;

        animator.SetTrigger(isFinishedHash);
        player.LevelFinish();
    }
}
