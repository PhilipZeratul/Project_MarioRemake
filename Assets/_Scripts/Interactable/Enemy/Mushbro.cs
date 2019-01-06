using UnityEngine;


public class Mushbro : GroundEnemy
{
    public PointText pointText;
    
    private readonly int triggerDieHash = Animator.StringToHash("TriggerDie");


    protected override void TreadDie()
    {
        Debug.LogFormat("Enemy: {0} Die()", gameObject.name);
        gameManager.AddScore(100);
        PointText pointTextInstance = Instantiate(pointText, transform.position, new Quaternion());
        pointTextInstance.SetScore(100);
        animator.SetTrigger(triggerDieHash);
    }

    protected override void HitDie()
    {
        //TODO:~ Need left or right information, flip and up and fall
    }

    public void Finish()
    {
        Destroy(gameObject);
    }
}
