using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PhysicsObject))]
[RequireComponent(typeof(Collider2D))]
public class PowerupMushroom : MonoBehaviour, IInteractableObject
{
    public float moveSpeed = 3f;
    public float totalTime = 2f;

    private GameManager gameManager;
    private bool isActive = false;
    private PhysicsObject physicsObject;
    private bool isMovingRight = true;
    private float moveTime;
    private bool isTimeup;
    private Collider2D collider2d;

    private Vector3 startPosition;
    private Vector3 endPosition;


    private void Start()
    {
        gameManager = GameObject.FindWithTag(Constants.TagNames.GameManager).GetComponent<GameManager>();

        physicsObject = GetComponent<PhysicsObject>();
        if (physicsObject != null)
            physicsObject.hitEvent += HitSelfReaction;

        collider2d = GetComponent<Collider2D>();

        startPosition = transform.position;
        endPosition = startPosition + new Vector3(0f, 0.6f, 0f);

        StartCoroutine(Spawn());
    }

    private void FixedUpdate()
    {
        if (physicsObject != null && isActive)
        {
            if (isMovingRight)
                physicsObject.Move(moveSpeed);
            else
                physicsObject.Move(-moveSpeed);
        }
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

    private void HitSelfReaction(GameObject target, Constants.HitDirection dir)
    {
        if (target.tag == Constants.TagNames.Player)
        {
            IsHit(target.transform.root.gameObject, dir);
        }

        if ((isMovingRight && dir == Constants.HitDirection.Left) ||
            (!isMovingRight && dir == Constants.HitDirection.Right))
        {
            isMovingRight = !isMovingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator Spawn()
    {
        collider2d.enabled = false;
        physicsObject.enabled = false;
        StartCoroutine(Counter());

        while (!isTimeup)
        {
            moveTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, moveTime / totalTime);
            yield return null;
        }
        collider2d.enabled = true;
        physicsObject.enabled = true;
        isActive = true;
    }

    private IEnumerator Counter()
    {
        isTimeup = false;
        yield return new WaitForSeconds(totalTime);
        isTimeup = true;
    }
}
