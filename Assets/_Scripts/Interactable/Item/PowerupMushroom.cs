using UnityEngine;
using Zenject;


public class PowerupMushroom : MonoBehaviour, IInteractableObject
{
    GameManager gameManager;


    [Inject]
    private void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void IsHit(GameObject source, Constants.HitDirection from)
    {
        PlayerController player = source.GetComponent<PlayerController>();

        if (player != null)
        {
            gameManager.AddScore(1000);
            player.ChangeToBig();
            Destroy(gameObject);
        }
    }
}
