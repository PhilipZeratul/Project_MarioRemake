using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float offset = 2f;


    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + offset, transform.position.y, transform.position.z);
    }
}
