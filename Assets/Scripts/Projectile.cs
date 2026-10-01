using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 direction;
    public float speed = 8f;

    [Header("Hit Detection")]
    public float hitDistance = 0.3f;

    [Header("Lifetime")]
    public float lifetime = 5f;

    private bool active = true;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!active)
            return;
        
        transform.position +=
            (Vector3)(
                direction.normalized *
                speed *
                Time.deltaTime
            );//it says here my MULTIPLICATION is ineffcient??? like how? am i stupid(yes duh, had to consult GPT honestly)

        CheckForEnemyHit();
    }

    void CheckForEnemyHit()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance <= hitDistance)
            {
                active = false;

                enemy.Kill();

                Destroy(gameObject);
                
                Debug.Log("Enemy has been killed");

                return;
            }
        }
    }
}