using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierPathRenderer : MonoBehaviour
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

    [Header("Visual")]
    public int segments = 40;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer =
            GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawPath();
    }

    void DrawPath()
    {
        if (lineRenderer == null)
            return;

        if (spawnPoint == null ||
            target == null ||
            controlPoint1 == null)
        {
            return;
        }

        if (pathType == PathType.Cubic &&
            controlPoint2 == null)
        {
            return;
        }

        lineRenderer.positionCount =
            segments + 1;

        for (int i = 0;
             i <= segments;
             i++)
        {
            float t =
                (float)i / segments;

            Vector3 position =
                GetBezierPosition(t);

            lineRenderer.SetPosition(
                i,
                position
            );
        }
    }

    Vector3 GetBezierPosition(float t)
    {
        if (pathType ==
            PathType.Quadratic)
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
        float oneMinusT =
            1f - t;

        return
            oneMinusT *
            oneMinusT *
            p0
            +
            2f *
            oneMinusT *
            t *
            p1
            +
            t *
            t *
            p2;
    }

    Vector3 CubicBezier(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t)
    {
        float oneMinusT =
            1f - t;

        return
            oneMinusT *
            oneMinusT *
            oneMinusT *
            p0
            +
            3f *
            oneMinusT *
            oneMinusT *
            t *
            p1
            +
            3f *
            oneMinusT *
            t *
            t *
            p2
            +
            t *
            t *
            t *
            p3;
    }
}