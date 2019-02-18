using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GenerateCurve : MonoBehaviour {

    public float radius = 0.1f;
    public Vector3 endPosition = new Vector3(5f, 0f, 0f);
    public Vector2 maxRandomAmplitude = new Vector2(0.2f, 2.0f);
    public Vector2 minRandomAmplitude = new Vector2(-0.2f, 0.7f);
    public float footHeight = 1.5f;
    private float length = 0f;

    [Range(0.05f, 2f)]
    public float distanceBetweenPoints = 0.05f;

    [Range(4, 15)]
    public int quality = 10;

    private List<Vector3> verts;
    private List<int> tris;
    private List<Vector3> norms;

    private List<Vector3> randomPoints;
    private bool randomPointsGenerated = false;

    [Range(2, 16)]
    public int maxRandomPoints = 10;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void OnDrawGizmosSelected () {
        Gizmos.color = Color.green;
 
        Vector3 scale = transform.lossyScale;

        if(randomPointsGenerated && randomPoints.Count >= 4)
        {
            DrawGizmoSpline(randomPoints[0], randomPoints[1], randomPoints[2], randomPoints[3], scale);

            for(int i = 3; i < randomPoints.Count - 2; i+=2)
            {
                Vector3 startPos = randomPoints[i];
                Vector3 splinePos1 = startPos + (startPos - randomPoints[i - 1]);
                Vector3 splinePos2 = randomPoints[i + 1];
                Vector3 endPos = randomPoints[i + 2];

                DrawGizmoSpline(startPos, splinePos1, splinePos2, endPos, scale);
            }
        }
	}

    void DrawGizmoSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 scale)
    {
        p0.Scale(scale);
        p1.Scale(scale);
        p2.Scale(scale);
        p3.Scale(scale);

        for (float y = 0f; y < 1f; y += distanceBetweenPoints)
        {
            Vector3 startPos = getCubicBezierPoint(p0, p1, p2, p3, y);
            Vector3 endPos = getCubicBezierPoint(p0, p1, p2, p3, y + distanceBetweenPoints);

            Vector3 velocity = (endPos - startPos).normalized;
            Vector3 up = Vector3.Cross(Vector3.Cross(velocity, Vector3.up), velocity).normalized;
            Matrix4x4 rot = Matrix4x4.LookAt(Vector3.zero, velocity, up);

            Gizmos.color = Color.green;

            for (int x = 0; x < quality; x++)
            {
                float stepSize = (1.0f / (quality - 1f));
                float anglePoint = stepSize * x;
                Vector3 startPoint = new Vector3(Mathf.Sin(anglePoint * Mathf.PI * 2f) * radius, Mathf.Cos(anglePoint * Mathf.PI * 2f) * radius, 0f);
                Vector3 endPoint = new Vector3(Mathf.Sin((anglePoint + stepSize) * Mathf.PI * 2f) * radius, Mathf.Cos((anglePoint + stepSize) * Mathf.PI * 2f) * radius, 0f);
                startPoint = rot.MultiplyPoint3x4(startPoint);
                endPoint = rot.MultiplyPoint3x4(endPoint);

                startPoint.Scale(scale);
                endPoint.Scale(scale);

                Gizmos.DrawLine(transform.position + startPos + startPoint, transform.position + startPos + endPoint);
            }
        }
    }

    public void CalculateRandomPoints()
    {
        randomPoints = new List<Vector3>();

        randomPoints.Add(Vector3.zero);
        randomPoints.Add(Vector3.up * footHeight);

        length = endPosition.magnitude;

        Vector3 direction = endPosition / maxRandomPoints;

        Vector3 side = Vector3.Cross(endPosition.normalized, Vector3.up).normalized;

        for(int i = 1; i < maxRandomPoints - 1; i++)
        {
            Vector3 point = direction * i;
            point += Vector3.up * Random.Range(minRandomAmplitude.y, maxRandomAmplitude.y);
            point += side * Random.Range(minRandomAmplitude.x, maxRandomAmplitude.x);
            randomPoints.Add(point);
            //randomPoints.Add(new Vector3(distance * i, Random.Range(0.7f, 2.0f), Random.Range(-0.5f, 0.5f)));
        }

        //randomPoints.Add(new Vector3(length, 1f, 0f));
        //randomPoints.Add(new Vector3(length, 0f, 0f));
        randomPoints.Add(endPosition + Vector3.up * footHeight);
        randomPoints.Add(endPosition);

        randomPointsGenerated = true;
    }

    public void BuildMeshBezier()
    {
        //CalculateRandomPoints();

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogError("No Meshfilter Connected!");
            return;
        }
        Mesh mesh = new Mesh();
        verts = new List<Vector3>();
        tris = new List<int>();
        norms = new List<Vector3>();

        if (randomPoints.Count >= 4)
        {
            DrawCylindersBySpline(randomPoints[0], randomPoints[1], randomPoints[2], randomPoints[3], 0);

            for (int i = 3; i < randomPoints.Count - 2; i += 2)
            {
                Vector3 startPos = randomPoints[i];
                Vector3 splinePos1 = startPos + (startPos - randomPoints[i - 1]);
                Vector3 splinePos2 = randomPoints[i + 1];
                Vector3 endPos = randomPoints[i + 2];

                DrawCylindersBySpline(startPos, splinePos1, splinePos2, endPos, i);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.SetNormals(norms);

        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        ClearMeshes();

        mf.sharedMesh = mesh;

        if (GetComponent<MeshCollider>())
        {
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    void DrawCylindersBySpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int spline)
    {
        float dist = Mathf.Abs(length / (randomPoints.Count / 4f)) / distanceBetweenPoints;
        dist = 1.0f / dist;

        for (float y = 0f; y < 1f; y += dist)
        {
            if(y + dist > 1f)
            {
                dist = 1.0f - y;
            }

            Vector3 startPos = getCubicBezierPoint(p0, p1, p2, p3, y);
            Vector3 endPos = getCubicBezierPoint(p0, p1, p2, p3, y + dist);

            Vector3 startVelocity = (endPos - startPos).normalized;

            DrawCylinder(startPos, endPos, startVelocity);
        }
    }

    void DrawCylinder(Vector3 startPos, Vector3 endPos, Vector3 startVelocity)
    {
        Vector3 up1 = Vector3.Cross(Vector3.Cross(startVelocity, Vector3.up), startVelocity).normalized;
        Matrix4x4 rot1 = Matrix4x4.LookAt(Vector3.zero, startVelocity, up1);

        List<Vector3> points = new List<Vector3>();

        for (int x = 0; x < quality; x++)
        {
            float stepSize = (1.0f / (quality - 1f));
            float anglePoint = stepSize * x;
            Vector3 startPoint = new Vector3(Mathf.Sin(anglePoint * Mathf.PI * 2f) * radius, Mathf.Cos(anglePoint * Mathf.PI * 2f) * radius, 0f);

            Vector3 startPoint1 = rot1.MultiplyPoint3x4(startPoint);

            points.Add(startPos + startPoint1);
        }

        AddRing(startPos, points);
    }

    void AddRing(Vector3 startPos, List<Vector3> points)
    {
        int vertCount1 = verts.Count - points.Count;
        int vertCount2 = verts.Count;
        for (int i = 0; i < points.Count; i++)
        {
            verts.Add(points[i]);
            norms.Add((points[i] - startPos).normalized);
        }

        if(vertCount1 >= 0)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                tris.Add(vertCount1 + i);
                tris.Add(vertCount2 + i);
                tris.Add(vertCount1 + i + 1);

                tris.Add(vertCount1 + i + 1);
                tris.Add(vertCount2 + i);
                tris.Add(vertCount2 + i + 1);
            }
        }
    }

    Vector3 getQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
    }

    Vector3 getCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

    public bool hasRandomPoints()
    {
        return randomPointsGenerated;
    }

    public void ClearMeshes()
    {
        if (GetComponent<MeshFilter>().mesh)
        {
            Destroy(GetComponent<MeshFilter>().mesh);
        }
        if (GetComponent<MeshCollider>().sharedMesh)
        {
            Destroy(GetComponent<MeshCollider>().sharedMesh);
        }
    }
}
