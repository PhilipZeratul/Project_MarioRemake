using UnityEngine;


public class ActivationMark : MonoBehaviour
{
    public EnemyBase[] activationObjects;

    private bool isActive = false;
    private Camera mainCamera;
    private Vector3 viewportPosition;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isActive)
        {
            viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPosition.x > 0f && viewportPosition.x <= 1f)
            {
                isActive = true;

                if (activationObjects != null)
                    for (int i = 0; i < activationObjects.Length; i++)
                        if (activationObjects[i] != null)
                            activationObjects[i].SetIsActive(true);
            }
        }
        else
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (activationObjects != null)
            for (int i = 0; i < activationObjects.Length; i++)           
                if (activationObjects[i] != null)
                    Debug.DrawLine(transform.position, activationObjects[i].transform.position, Color.red);            
    }
}
