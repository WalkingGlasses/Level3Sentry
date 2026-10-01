using UnityEngine;

public class TurretController : MonoBehaviour
{
    public enum TurretType
    {
        Flame,
        Sniper,
        Shotgun
    }
    //slight changes have been made here compared to old script, too annoyed to note them in detail

    [Header("Turret")]
    public TurretType turretType;

    [Header("Detection")]
    public float range = 5f;

    [Header("Cone")]
    [Range(1f, 180f)]
    public float coneAngle = 60f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float projectileSpeed = 8f;

    [Header("Flame")]
    public float flameFireRate = 0.15f;

    [Header("Shotgun")]
    public float shotgunFireRate = 1.5f;
    public int shotgunPellets = 5;
    public float shotgunSpread = 30f;

    [Header("Sniper")]
    public float sniperLineWidth = 0.3f;

    private float nextFireTime;

    private bool active = true;

    private bool sniperHasFired = false;

    private LineRenderer lineRenderer;
    

    void Start()
    {
        lineRenderer =
            GetComponent<LineRenderer>();

        DrawRange();
    }
    
    void Update()
    {
        DrawRange();

        if (!active)
            return;

        switch (turretType)
        {
            case TurretType.Flame:
                FlameTurret();
                break;

            case TurretType.Sniper:
                SniperTurret();
                break;

            case TurretType.Shotgun:
                ShotgunTurret();
                break;
        }
    }

    void FlameTurret()
    {
        Enemy target =
            FindEnemyInCone();

        if (target == null)
            return;

        if (Time.time >= nextFireTime)
        {
            Vector2 direction =
                GetDirectionToEnemy(target);

            FireProjectile(direction);

            nextFireTime =
                Time.time +
                flameFireRate;
        }
    }

    void SniperTurret()
    {
        Enemy target =
            FindEnemyInSightLine();

        if (target != null &&
            !sniperHasFired)
        {
            Vector2 direction =
                GetDirectionToEnemy(target);

            FireProjectile(direction);

            sniperHasFired = true;
        }

        // Allow the sniper to fire again
        // after the enemy leaves the line.
        if (target == null)
        {
            sniperHasFired = false;
        }
    }

    void ShotgunTurret()
    {
        Enemy target =
            FindEnemyInCone();

        if (target == null)
            return;

        if (Time.time >= nextFireTime)
        {
            FireShotgun(target);

            nextFireTime =
                Time.time +
                shotgunFireRate;
        }
    }

    Enemy FindEnemyInCone()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        Enemy closestEnemy = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            Vector2 toEnemy =
                (Vector2)enemy.transform.position -
                (Vector2)transform.position;

            float distance =
                toEnemy.magnitude;

            if (distance > range)
                continue;

            Vector2 directionToEnemy =
                toEnemy.normalized;

            Vector2 forward =
                transform.right;

            float dot =
                Vector2.Dot(
                    forward,
                    directionToEnemy
                );

            float angleLimit =
                Mathf.Cos(
                    coneAngle *
                    0.5f *
                    Mathf.Deg2Rad
                );

            if (dot < angleLimit)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    

    Enemy FindEnemyInSightLine()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        Enemy closestEnemy = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            Vector2 toEnemy =
                (Vector2)enemy.transform.position -
                (Vector2)transform.position;

            float distance =
                toEnemy.magnitude;

            if (distance > range)
                continue;

            Vector2 forward =
                transform.right;

            float forwardAmount =
                Vector2.Dot(
                    toEnemy.normalized,
                    forward
                );

            if (forwardAmount <= 0f)
                continue;

            float perpendicularDistance =
                Mathf.Abs(
                    toEnemy.x * forward.y -
                    toEnemy.y * forward.x
                );

            if (perpendicularDistance >
                sniperLineWidth)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }


    Vector2 GetDirectionToEnemy(
        Enemy enemy)
    {
        Vector2 difference =
            (Vector2)enemy.transform.position -
            (Vector2)firePoint.position;
        
        float angle =
            Mathf.Atan2(
                difference.y,
                difference.x
            );

        return new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ).normalized;
    }
    
    void FireProjectile(
        Vector2 direction)
    {
        GameObject projectile =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        Projectile projectileScript =
            projectile.GetComponent<Projectile>();

        projectileScript.direction =
            direction;

        projectileScript.speed =
            projectileSpeed;
    }

    void FireShotgun(Enemy target)
    {
        Vector2 direction =
            GetDirectionToEnemy(target);

        float centerAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        for (int i = 0;
             i < shotgunPellets;
             i++)
        {
            float t;

            if (shotgunPellets == 1)
            {
                t = 0f;
            }
            else
            {
                t =
                    (float)i /
                    (shotgunPellets - 1);
            }

            float angle =
                centerAngle +
                Mathf.Lerp(
                    -shotgunSpread,
                    shotgunSpread,
                    t
                );

            float radians =
                angle *
                Mathf.Deg2Rad;

            Vector2 pelletDirection =
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

            FireProjectile(
                pelletDirection
            );
        }
    }

    void DrawRange()
    {
        if (lineRenderer == null)
            return;

        if (turretType ==
            TurretType.Sniper)
        {
            DrawSniperLine();
        }
        else
        {
            DrawCone();
        }
    }

    void DrawSniperLine()
    {
        lineRenderer.positionCount = 2;

        Vector2 start =
            transform.position;

        Vector2 end =
            start +
            (Vector2)transform.right *
            range;

        lineRenderer.SetPosition(
            0,
            start
        );

        lineRenderer.SetPosition(
            1,
            end
        );
    }

    void DrawCone()
    {
        int segments = 30;

        lineRenderer.positionCount =
            segments + 2;

        Vector2 center =
            transform.position;

        // Actual Z rotation
        float centerAngle =
            transform.eulerAngles.z;

        lineRenderer.SetPosition(
            0,
            center
        );

        for (int i = 0;
             i <= segments;
             i++)
        {
            float t =
                (float)i /
                segments;

            float angle =
                centerAngle -
                coneAngle / 2f +
                coneAngle * t;

            float radians =
                angle *
                Mathf.Deg2Rad;

            Vector2 point =
                center +
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                ) *
                range;

            lineRenderer.SetPosition(
                i + 1,
                point
            );
        }
    }

    public void StopTurret()
    {
        active = false;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}