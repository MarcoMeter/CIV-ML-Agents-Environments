using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCurve : MonoBehaviour {

    public float length = 5f;

    [Range(0.05f, 2f)]
    public float distanceBetweenPoints = 0.2f;
    public float amplitude = 1f;
    public float waveLength = 1f;
    public float offset = 0f;

    [Range(4, 15)]
    public int quality = 10;

    private List<Vector3> verts;
    private List<int> tris;
    private List<Vector3> norms;

    private List<Vector3> randomPoints;
    public int maxRandomPoints = 10;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnDrawGizmosSelected () {
        Gizmos.color = Color.green;

        float distance = distanceBetweenPoints / randomPoints.Count;

        if(randomPoints.Count >= 4)
        {
            //DrawGizmoSpline(randomPoints[0], randomPoints[1], randomPoints[2], randomPoints[3]);

            for(int i = 3; i < randomPoints.Count - 2; i+=2)
            {
                Vector3 startPos = randomPoints[i];
                Vector3 splinePos1 = startPos + (startPos - randomPoints[i - 1]);
                Vector3 splinePos2 = randomPoints[i + 1];
                Vector3 endPos = randomPoints[i + 2];
                //DrawGizmoSpline(startPos, splinePos1, splinePos2, endPos);
            }
        }


        /*for (float i = 0f; i < length; i += distanceBetweenPoints)
        {
            Vector3 startPos = new Vector3(i, Mathf.Sin((i) * waveLength + offset) * amplitude, 0f);
            Vector3 endPos = new Vector3(i + distanceBetweenPoints, Mathf.Sin((i + distanceBetweenPoints) * waveLength + offset) * amplitude, 0f);

            Vector3 velocity = (endPos - startPos).normalized;
            Vector3 up = Vector3.Cross(Vector3.Cross(velocity, Vector3.up), velocity).normalized;

            Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position + startPos, transform.position + startPos + velocity);
            //Gizmos.DrawLine(transform.position + startPos, transform.position + startPos + up);

            Matrix4x4 rot = Matrix4x4.LookAt(Vector3.zero, velocity, up);

            Gizmos.color = Color.green;

            for (int x = 0; x < quality; x++)
            {
                float stepSize = (1.0f / (quality - 1f));
                float anglePoint = stepSize * x;
                Vector3 startPoint = new Vector3(Mathf.Sin(anglePoint * Mathf.PI * 2f) * 0.1f, Mathf.Cos(anglePoint * Mathf.PI * 2f) * 0.1f, 0f);
                Vector3 endPoint = new Vector3(Mathf.Sin((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, Mathf.Cos((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, 0f);
                startPoint = rot.MultiplyPoint3x4(startPoint);
                endPoint = rot.MultiplyPoint3x4(endPoint);
                Gizmos.DrawLine(transform.position + startPos + startPoint, transform.position + startPos + endPoint);
            }

            //Gizmos.DrawLine(transform.position + startPos, transform.position + endPos);
        }*/

        Gizmos.color = Color.red;
	}

    void DrawGizmoSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {

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
                Vector3 startPoint = new Vector3(Mathf.Sin(anglePoint * Mathf.PI * 2f) * 0.1f, Mathf.Cos(anglePoint * Mathf.PI * 2f) * 0.1f, 0f);
                Vector3 endPoint = new Vector3(Mathf.Sin((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, Mathf.Cos((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, 0f);
                startPoint = rot.MultiplyPoint3x4(startPoint);
                endPoint = rot.MultiplyPoint3x4(endPoint);
                Gizmos.DrawLine(transform.position + startPos + startPoint, transform.position + startPos + endPoint);
            }
        }
    }

    public void CalculateRandomPoints()
    {
        randomPoints = new List<Vector3>();

        randomPoints.Add(Vector3.zero);
        randomPoints.Add(Vector3.up * 1f);

        float distance = length / maxRandomPoints;

        for(int i = 1; i < maxRandomPoints - 1; i++)
        {
            randomPoints.Add(new Vector3(distance * i, Random.Range(0.7f, 2.0f), Random.Range(-0.5f, 0.5f)));
        }

        randomPoints.Add(new Vector3(length, 1f, 0f));
        randomPoints.Add(new Vector3(length, 0f, 0f));
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
        verts = new List<Vector3>();
        tris = new List<int>();
        norms = new List<Vector3>();

        Mesh mesh = new Mesh();

        float distance = distanceBetweenPoints / randomPoints.Count;

        lastVelocity = Vector3.up;

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

        mf.mesh = mesh;
    }

    private Vector3 lastVelocity = Vector3.zero;

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

            Vector3 nextPos = getCubicBezierPoint(p0, p1, p2, p3, y + (dist * 2f));

            if(y + (dist * 2f) > 1f)
            {
                nextPos = endPos + (endPos - startPos);
                lastVelocity = (nextPos - endPos).normalized;
            }

            Vector3 startVelocity = (endPos - startPos).normalized;
            Vector3 endVelocity = (nextPos - endPos).normalized;

            if(y <= 0f)
            {
                //startVelocity = lastVelocity;
            }

            DrawCylinder(startPos, endPos, startVelocity, endVelocity);
        }
    }

    int getFromRandomPoint(float xAxis)
    {
        for(int i = 0; i < randomPoints.Count; i++)
        {
            if(xAxis >= randomPoints[i].x)
            {
                return i;
            }
        }

        return -1;
    }
    
    public void BuildMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if(mf == null)
        {
            Debug.LogError("No Meshfilter Connected!");
            return;
        }
        verts = new List<Vector3>();
        tris = new List<int>();
        norms = new List<Vector3>();

        Mesh mesh = new Mesh();

        for (float i = 0f; i < length; i += distanceBetweenPoints)
        {
            Vector3 startPos = new Vector3(i, Mathf.Sin((i) * waveLength + offset) * amplitude, 0f);
            Vector3 endPos = new Vector3(i + distanceBetweenPoints, Mathf.Sin((i + distanceBetweenPoints) * waveLength + offset) * amplitude, 0f);
            Vector3 nextPos = new Vector3(i + (distanceBetweenPoints * 2f), Mathf.Sin((i + (distanceBetweenPoints * 2f)) * waveLength + offset) * amplitude, 0f);
            //DrawCylinder(startPos, endPos, nextPos);
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.SetNormals(norms);

        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        mf.mesh = mesh;
    }

    void DrawCylinder(Vector3 startPos, Vector3 endPos, Vector3 startVelocity, Vector3 endVelocity)
    {
        //Vector3 velocity1 = (endPos - startPos).normalized;
        Vector3 up1 = Vector3.Cross(Vector3.Cross(startVelocity, Vector3.up), startVelocity).normalized;
        Matrix4x4 rot1 = Matrix4x4.LookAt(Vector3.zero, startVelocity, up1);

        //Vector3 velocity2 = (nextPos - endPos).normalized;
        Vector3 up2 = Vector3.Cross(Vector3.Cross(endVelocity, Vector3.up), endVelocity).normalized;
        Matrix4x4 rot2 = Matrix4x4.LookAt(Vector3.zero, endVelocity, up2);

        List<Vector3> points = new List<Vector3>();

        for (int x = 0; x < quality; x++)
        {
            float stepSize = (1.0f / (quality - 1f));
            float anglePoint = stepSize * x;
            Vector3 startPoint = new Vector3(Mathf.Sin(anglePoint * Mathf.PI * 2f) * 0.1f, Mathf.Cos(anglePoint * Mathf.PI * 2f) * 0.1f, 0f);
            Vector3 endPoint = new Vector3(Mathf.Sin((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, Mathf.Cos((anglePoint + stepSize) * Mathf.PI * 2f) * 0.1f, 0f);

            Vector3 startPoint1 = rot1.MultiplyPoint3x4(startPoint);
            Vector3 endPoint1 = rot1.MultiplyPoint3x4(endPoint);

            Vector3 startPoint2= rot2.MultiplyPoint3x4(startPoint);
            Vector3 endPoint2  = rot2.MultiplyPoint3x4(endPoint);

            points.Add(startPos + startPoint1);
            //points.Add(endPoint1);

            //DrawQuad(startPos, endPos, startPos + startPoint1, startPos + endPoint1, endPos + startPoint2, endPos + endPoint2);
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

    void DrawQuad(Vector3 startPos, Vector3 endPos, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        int vertsCount = verts.Count;
        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        verts.Add(p4);

        norms.Add((p1 - startPos).normalized);
        norms.Add((p2 - startPos).normalized);
        norms.Add((p3 - endPos).normalized);
        norms.Add((p4 - endPos).normalized);

        tris.Add(vertsCount);
        tris.Add(vertsCount + 2);
        tris.Add(vertsCount + 1);

        tris.Add(vertsCount + 3);
        tris.Add(vertsCount + 1);
        tris.Add(vertsCount + 2);
    }

    Vector3 getQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
    }

    Vector3 getCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }
}
