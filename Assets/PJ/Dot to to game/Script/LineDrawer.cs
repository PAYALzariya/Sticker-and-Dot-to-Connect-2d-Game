// LineDrawer.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    public LineRenderer lr;
    private List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.useWorldSpace = true;
        lr.numCapVertices = 8;
    }

    public void StartLine(Vector3 start)
    {
        points.Clear();
        points.Add(start);
        lr.positionCount = 1;
        lr.SetPosition(0, start);
    }

    public void AppendPoint(Vector3 p)
    {
        // only append if far enough to reduce points
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], p) > 0.02f)
        {
            points.Add(p);
            lr.positionCount = points.Count;
            lr.SetPosition(points.Count - 1, p);
        }
    }

    public void SnapTo(Vector3 p)
    {
        AppendPoint(p);
    }

    public void ClearLine()
    {
        points.Clear();
        lr.positionCount = 0;
    }
}
