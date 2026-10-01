using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum PathType
    {
        Quadratic,
        Cubic
    }

    [Header("Path")]
    public PathType pathType;

    public Transform spawnPoint;
    public Transform target;

    public Transform controlPoint1;
    public Transform controlPoint2;

    [Header("Movement")]
    public float speed = 0.2f;

    [Header("Reward")]
    public int coinReward = 10;

    private float t = 0f;

    private bool alive = true;

    void Update()
    {
        if (!alive)
            return;
        
        t += speed * Time.deltaTime;

        if (t >= 1f)
        {
            t = 1f;

            transform.position = GetBezierPosition(t);

            ReachBase();

            return;
        }

        transform.position = GetBezierPosition(t);
    }
    
    Vector3 GetBezierPosition(float t)
    {
        if (pathType == PathType.Quadratic)
        {
            return QuadraticBezier(
                spawnPoint.position,
                controlPoint1.position,
                target.position,
                t
            );
        }

        return CubicBezier(
            spawnPoint.position,
            controlPoint1.position,
            controlPoint2.position,
            target.position,
            t
        );
    }
    
    Vector3 QuadraticBezier(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        float t)
    {
        float oneMinusT = 1f - t;

        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    Vector3 CubicBezier(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t)
    {
        float oneMinusT = 1f - t;

        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    void ReachBase()
    {
        alive = false;

        if (BaseHealth.Instance != null)
        {
            BaseHealth.Instance.TakeDamage(1);
        }

        Destroy(gameObject);
    }

    public void Kill()
    {
        if (!alive)
            return;

        alive = false;

        // salary time
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.SpawnCoin(
                transform.position,
                coinReward
            );
        }

        Destroy(gameObject);
    }
}