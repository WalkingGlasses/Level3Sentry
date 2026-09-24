using UnityEngine;

public class TurretController : MonoBehaviour
{
    public enum TurretType
    {
        Flame,
        Sniper,
        Shotgun
    }
    //Like ALL Turret shenanigens are here

    [Header("Turret")]
    public TurretType turretType;

    public Transform player;

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

    private float nextFireTime;
    private bool active = true;
    private bool sniperHasFired = false;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        DrawRange();
    }

    void Update()
    {
        // Draws range constantly so it updates with rotation(i broke it before and didnt use transform.right and made it stuck to the right HAHAHAHAHHA
        DrawRange();

        if (!active || player == null)
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
        if (IsInsideCone())
        {
            if (Time.time >= nextFireTime)
            {
                FireProjectile(
                    GetDirectionToPlayer()
                );

                nextFireTime = Time.time + flameFireRate;
            }
        }
    }

    void SniperTurret()
    {
        bool insideSightLine = IsInsideSightLine();

        if (insideSightLine && !sniperHasFired)
        {
            FireProjectile(
                GetDirectionToPlayer()
            );

            sniperHasFired = true;
        }

        if (!insideSightLine)
        {
            sniperHasFired = false;
        }
    }

    void ShotgunTurret()
    {
        if (IsInsideCone())
        {
            if (Time.time >= nextFireTime)
            {
                FireShotgun();

                nextFireTime =
                    Time.time + shotgunFireRate;
            }
        }
    }

    bool IsInsideCone()//checks if... its inside a cone
    {
        Vector2 toPlayer =
            (Vector2)player.position -
            (Vector2)transform.position;

        float distance = toPlayer.magnitude;

        if (distance > range)
            return false;

        Vector2 directionToPlayer =
            toPlayer.normalized;

        // Turret's actual rotated right direction
        Vector2 forward = transform.right;

        // DOT PRODUCT
        float dot =
            Vector2.Dot(
                forward,
                directionToPlayer
            );

        float angleLimit =
            Mathf.Cos(
                coneAngle *
                0.5f *
                Mathf.Deg2Rad
            );

        return dot >= angleLimit;
    }

    bool IsInsideSightLine()//like the cone one but for the sniper one since it isnt a cone but a line
    {
        Vector2 toPlayer =
            (Vector2)player.position -
            (Vector2)transform.position;

        float distance = toPlayer.magnitude;

        if (distance > range)
            return false;

        // Turret's actual rotated right direction because last time i messed it up and gave my turrets stiff neck
        Vector2 forward = transform.right;
        
        float forwardAmount =
            Vector2.Dot(
                toPlayer.normalized,
                forward
            );

        if (forwardAmount <= 0f)
            return false;

        float perpendicularDistance =
            Mathf.Abs(
                toPlayer.x * forward.y -
                toPlayer.y * forward.x
            );

        return perpendicularDistance <= 0.3f;
    }

    // =========================
    // DIRECTION / ATAN2
    // =========================

    Vector2 GetDirectionToPlayer()
    {
        Vector2 difference =
            (Vector2)player.position -
            (Vector2)firePoint.position;

        // ATAN2
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


    void FireProjectile(Vector2 direction)
    {
        GameObject projectile =
            Instantiate(//i am scare of the timre when we somehow dont need this HOW
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        Projectile projectileScript =
            projectile.GetComponent<Projectile>();

        projectileScript.direction = direction;
        projectileScript.speed = projectileSpeed;
    }
    

    void FireShotgun()
    {
        Vector2 direction =
            GetDirectionToPlayer();

        float centerAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        for (int i = 0; i < shotgunPellets; i++)
        {
            float t;

            if (shotgunPellets == 1)
                t = 0;
            else
                t =
                    (float)i /
                    (shotgunPellets - 1);

            float angle =
                centerAngle +
                Mathf.Lerp(
                    -shotgunSpread,
                    shotgunSpread,
                    t
                );

            float radians =
                angle * Mathf.Deg2Rad;

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
    

    void DrawRange()//calls range drawing for linerenderer for both the cone turrets and the one sniper turret
    {
        if (lineRenderer == null)
            return;

        if (turretType == TurretType.Sniper)
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

        // Uses the turret's actual rotation
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

        // Uses the turret's actual Z rotation
        float centerAngle =
            transform.eulerAngles.z;

        lineRenderer.SetPosition(
            0,
            center
        );

        for (int i = 0; i <= segments; i++)
        {
            float t =
                (float)i / segments;

            float angle =
                centerAngle -
                coneAngle / 2f +
                coneAngle * t;

            float radians =
                angle * Mathf.Deg2Rad;

            Vector2 point =
                center +
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                ) * range;

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