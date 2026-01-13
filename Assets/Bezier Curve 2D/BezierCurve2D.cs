using UnityEngine;

/// <summary>
/// BezierCurve2D_UI
/// ------------------------------------------------------------
/// Draws a 2D Bezier curve on UI canvas using UILineRenderer
/// based on a set of RectTransform control points.
///
/// Features:
/// - Supports any order Bezier curve (2+ control points)
/// - Uses De Casteljau's algorithm for curve evaluation
/// - Works in 2D UI space (x, y only - z is always 0)
///
/// Requirements:
/// - A UILineRenderer component must be attached
/// - At least 2 control points (RectTransforms) must be assigned
///
/// Use cases:
/// - UI path visualization
/// - Connection lines between UI elements
/// - Menu transitions
/// - UI progress paths
/// ------------------------------------------------------------
/// </summary>
[RequireComponent(typeof(UILineRenderer))]
public class BezierCurve2D : MonoBehaviour
{
    /// <summary>
    /// Control points that define the Bezier curve.
    /// The order of the curve is (points.Length - 1).
    /// </summary>
    [SerializeField]
    private RectTransform[] points;

    /// <summary>
    /// Number of segments used to approximate the curve.
    /// Higher values result in smoother curves.
    /// </summary>
    [SerializeField]
    private int resolution = 50;

    /// <summary>
    /// UILineRenderer used to visually draw the curve.
    /// </summary>
    private UILineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();

        if (points == null || points.Length < 2)
        {
            Debug.LogWarning("[BezierCurve2D_UI] At least two control points are required.");
            return;
        }

        DrawCurve();
    }

    /// <summary>
    /// Generates the Bezier curve and assigns the calculated
    /// positions to the UILineRenderer.
    /// </summary>
    private void DrawCurve()
    {
        if (points.Length < 2)
        {
            Debug.LogError("[BezierCurve2D_UI] Cannot draw curve with less than two points.");
            return;
        }

        // Allocate array for curve points
        Vector2[] curvePoints = new Vector2[resolution + 1];

        // Sample points along the curve
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            curvePoints[i] = CalculateBezierPoint(t, points);
        }

        // Assign positions to UILineRenderer
        lineRenderer.points = curvePoints;
        lineRenderer.SetVerticesDirty();
    }

    /// <summary>
    /// Calculates a single point on the Bezier curve at
    /// parameter t using De Casteljau's algorithm.
    /// </summary>
    /// <param name="t">
    /// Interpolation parameter (0–1).
    /// 0 = first control point, 1 = last control point.
    /// </param>
    /// <param name="controlPoints">
    /// Array of RectTransforms used as Bezier control points.
    /// </param>
    /// <returns>
    /// The calculated point on the Bezier curve (2D, z = 0).
    /// </returns>
    private Vector2 CalculateBezierPoint(float t, RectTransform[] controlPoints)
    {
        // Copy control point positions (only x and y)
        Vector2[] tempPoints = new Vector2[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            tempPoints[i] = controlPoints[i].anchoredPosition;
        }

        // De Casteljau's algorithm
        for (int j = 1; j < controlPoints.Length; j++)
        {
            for (int i = 0; i < controlPoints.Length - j; i++)
            {
                tempPoints[i] = Vector2.Lerp(tempPoints[i], tempPoints[i + 1], t);
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            lineRenderer = GetComponent<UILineRenderer>();
            if (lineRenderer != null && points != null && points.Length >= 2)
            {
                DrawCurve();
            }
        }
    }
#endif
}