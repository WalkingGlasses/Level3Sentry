using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 8f;
    public float hitDistance = 0.4f;
    public float lifetime = 5f;

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction.normalized * speed * Time.deltaTime);

        if (player == null)
            return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        if (distance <= hitDistance)
        {
            GameManager.Instance.PlayerHit();
        }
    }
}