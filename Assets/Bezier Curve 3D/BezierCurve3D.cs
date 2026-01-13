using UnityEngine;

/// <summary>
/// BezierCurve3D
/// ------------------------------------------------------------
/// Draws a 3D Bezier curve using a LineRenderer based on
/// a set of Transform control points.
///
/// Features:
/// - Supports any order Bezier curve (2+ control points)
/// - Uses De Casteljau’s algorithm for curve evaluation
/// - Works in full 3D space
///
/// Requirements:
/// - A LineRenderer component must be attached
/// - At least 2 control points must be assigned
///
/// Use cases:
/// - Path visualization
/// - Camera rails
/// - Projectile arcs
/// - Spline-based movement previews
/// ------------------------------------------------------------
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve3D : MonoBehaviour
{
    /// <summary>
    /// Control points that define the Bezier curve.
    /// The order of the curve is (points.Length - 1).
    /// </summary>
    [SerializeField]
    private Transform[] points;

    /// <summary>
    /// Number of segments used to approximate the curve.
    /// Higher values result in smoother curves.
    /// </summary>
    [SerializeField]
    private int resolution = 50;

    /// <summary>
    /// LineRenderer used to visually draw the curve.
    /// </summary>
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (points == null || points.Length < 2)
        {
            Debug.LogWarning("[BezierCurve3D] At least two control points are required.");
            return;
        }

        DrawCurve();
    }

    /// <summary>
    /// Generates the Bezier curve and assigns the calculated
    /// positions to the LineRenderer.
    /// </summary>
    private void DrawCurve()
    {
        if (points.Length < 2)
        {
            Debug.LogError("[BezierCurve3D] Cannot draw curve with less than two points.");
            return;
        }

        // Allocate array for curve points
        Vector3[] curvePoints = new Vector3[resolution + 1];

        // Sample points along the curve
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            curvePoints[i] = CalculateBezierPoint(t, points);
        }

        // Assign positions to LineRenderer
        lineRenderer.positionCount = curvePoints.Length;
        lineRenderer.SetPositions(curvePoints);
    }

    /// <summary>
    /// Calculates a single point on the Bezier curve at
    /// parameter t using De Casteljau’s algorithm.
    /// </summary>
    /// <param name="t">
    /// Interpolation parameter (0–1).
    /// 0 = first control point, 1 = last control point.
    /// </param>
    /// <param name="controlPoints">
    /// Array of Transforms used as Bezier control points.
    /// </param>
    /// <returns>
    /// The calculated point on the Bezier curve.
    /// </returns>
    private Vector3 CalculateBezierPoint(float t, Transform[] controlPoints)
    {
        // Copy control point positions
        Vector3[] tempPoints = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            tempPoints[i] = controlPoints[i].position;
        }

        // De Casteljau’s algorithm
        for (int j = 1; j < controlPoints.Length; j++)
        {
            for (int i = 0; i < controlPoints.Length - j; i++)
            {
                tempPoints[i] = Vector3.Lerp(tempPoints[i], tempPoints[i + 1], t);
            }
        }

        return tempPoints[0];
    }


    /// <summary>
    /// Call this to manually refresh the curve.
    /// Useful when control points move at runtime.
    /// </summary>
    public void UpdateCurve()
    {
        DrawCurve();
    }
}
