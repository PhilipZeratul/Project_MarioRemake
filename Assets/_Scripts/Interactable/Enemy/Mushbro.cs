using UnityEngine;


public class Mushbro : GroundEnemy
{
    public PointText pointText;
    
    private readonly int triggerDieHash = Animator.StringToHash("TriggerDie");


    protected override void Die()
    {
        Debug.LogFormat("Enemy: {0} Die()", gameObject.name);
        gameManager.AddScore(100);
        PointText pointTextInstance = Instantiate(pointText, transform.position, new Quaternion());
        pointTextInstance.SetScore(100);

        //TODO~: Set Score
        animator.SetTrigger(triggerDieHash);
    }

    public void Finish()
    {
        Destroy(gameObject);
    }
}
