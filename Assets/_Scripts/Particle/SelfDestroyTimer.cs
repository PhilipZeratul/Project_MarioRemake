using UnityEngine;


public class SelfDestroyTimer : MonoBehaviour
{
    public float surviveTime = 2f;
    private float startTime;


    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - startTime > surviveTime)
            Destroy(gameObject);
    }
}
