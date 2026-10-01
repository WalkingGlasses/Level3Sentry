using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Quadratic Path")]
    public Transform quadraticSpawn;
    public Transform quadraticControl;
    public Transform target;

    [Header("Cubic Path")]
    public Transform cubicSpawn;
    public Transform cubicControl1;
    public Transform cubicControl2;

    [Header("Spawning")]
    public float spawnInterval = 3f;

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnQuadraticEnemy();
            SpawnCubicEnemy();

            nextSpawnTime =
                Time.time + spawnInterval;
        }
    }

    void SpawnQuadraticEnemy()
    {
        GameObject enemyObject =
            Instantiate(
                enemyPrefab,
                quadraticSpawn.position,
                Quaternion.identity
            );

        Enemy enemy =
            enemyObject.GetComponent<Enemy>();

        enemy.pathType =
            Enemy.PathType.Quadratic;

        enemy.spawnPoint =
            quadraticSpawn;

        enemy.controlPoint1 =
            quadraticControl;

        enemy.target =
            target;
    }
    

    void SpawnCubicEnemy()
    {
        GameObject enemyObject =
            Instantiate(
                enemyPrefab,
                cubicSpawn.position,
                Quaternion.identity
            );

        Enemy enemy =
            enemyObject.GetComponent<Enemy>();

        enemy.pathType =
            Enemy.PathType.Cubic;

        enemy.spawnPoint =
            cubicSpawn;

        enemy.controlPoint1 =
            cubicControl1;

        enemy.controlPoint2 =
            cubicControl2;

        enemy.target =
            target;
    }
}