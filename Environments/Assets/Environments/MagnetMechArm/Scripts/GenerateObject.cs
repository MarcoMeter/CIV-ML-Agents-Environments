using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

[RequireComponent(typeof(MeshFilter))]
public class GenerateObject : MonoBehaviour {

    public float height = 1f;
    public float connectorHeight = 0.8f;

    public int countX = 10;
    public int countZ = 10;

    public float offsetX = 1.5f;
    public float offsetZ = 1.5f;

    public float outerRadius = 1f;
    public float innerRadius = 0.9f;

    public int segments = 10;

    [Range(3, 30)]
    public int quality = 10;

    public Material mainMaterial;
    public Material magnetMaterial;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<int> triangles = new List<int>();

    public Agent agent;

    public int placedMagnets;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
        Randomize();
        GenerateFullObject();
        placedMagnets = 0;
    }

    public void Randomize()
    {
        height = Random.Range(0.5f, 1.5f);
        connectorHeight = height - Random.Range(0.0f, height - 0.1f);
        outerRadius = Random.Range(0.3f, 0.5f);
        innerRadius = outerRadius - Random.Range(0.01f, 0.08f);
        countX = Random.Range(3, 5);
        countZ = Random.Range(3, 5);
        offsetX = Random.Range(outerRadius * 2f, outerRadius * 10f);
        offsetZ = Random.Range(outerRadius * 2f, outerRadius * 10f);
    }

    public void PlaceMagnet()
    {
        placedMagnets += 1;

        if(placedMagnets >= countX * countZ)
        {
            agent.AddReward(5f);
            agent.Done();
        }
    }

    public void GenerateFullObject()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));


        for (int x = 0; x < countX; x++)
        {
            for(int y = 0; y < countZ; y++)
            {
                Vector3 pos = new Vector3(x * offsetX, 0f, y * offsetZ);
                GameObject obj = new GameObject();
                obj.AddComponent<MeshFilter>();
                obj.AddComponent<MeshRenderer>();
                obj.GetComponent<MeshRenderer>().sharedMaterial = mainMaterial;
                obj.GetComponent<MeshFilter>().mesh = GenerateCylinderMesh();
                obj.transform.parent = transform;
                obj.transform.localPosition = pos;
                createMagnet(obj.transform);

                if(x <= countX - 2)
                {
                    CreateConnector(pos + new Vector3(outerRadius + (offsetX - (outerRadius * 2f)) / 2f, connectorHeight / 2f, 0f), new Vector3(offsetX - (outerRadius * 2f) + (outerRadius - innerRadius) / 2f, connectorHeight, outerRadius - innerRadius));
                }

                if (y <= countZ - 2)
                {
                    CreateConnector(pos + new Vector3(0f, connectorHeight / 2f, outerRadius + (offsetZ - (outerRadius * 2f)) / 2f), new Vector3(outerRadius - innerRadius, connectorHeight, offsetZ - (outerRadius * 2f) + (outerRadius - innerRadius) / 2f));
                }
            }
        }

        CreateConnector(new Vector3((offsetX * (countX - 1)) / 2f, (outerRadius - innerRadius) / 2f, (offsetZ * (countZ - 1)) / 2f), new Vector3(offsetX * (countX - 1), outerRadius - innerRadius, offsetZ * (countZ - 1)));
    }

    GameObject createMagnet(Transform parent)
    {
        GameObject magnet = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        magnet.GetComponent<MeshRenderer>().sharedMaterial = magnetMaterial;
        magnet.GetComponent<MeshRenderer>().enabled = false;
        magnet.transform.parent = parent;
        magnet.transform.localPosition = new Vector3(0f, outerRadius - innerRadius + 0.1f, 0f);
        magnet.transform.localScale = new Vector3(innerRadius * 2f, 0.1f, innerRadius * 2f);

        Magnet magnetScript = magnet.AddComponent<Magnet>();
        magnetScript.agent = agent;
        magnetScript.generateObject = this;

        return magnet;
    }

    GameObject CreateConnector(Vector3 pos, Vector3 size)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().sharedMaterial = mainMaterial;
        cube.transform.parent = transform;
        cube.transform.localPosition = pos;
        cube.transform.localScale = size;

        return cube;
    }

    Mesh GenerateCylinderMesh()
    {
        Mesh mesh = new Mesh();

        vertices.Clear();
        triangles.Clear();
        normals.Clear();

        GenerateCylinder(Vector3.zero);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    void GenerateCylinder(Vector3 position)
    {
        GenerateCylinderSideCircle(outerRadius, position);
        GenerateCylinderSideCircle(innerRadius, position, true);
        GenerateCylinderVerticalCircle(innerRadius, outerRadius, position + Vector3.up * height, Vector3.up);
        GenerateCylinderVerticalCircle(innerRadius, outerRadius, position + Vector3.zero, Vector3.down, true);

        GenerateBase(innerRadius, Vector3.up * (outerRadius - innerRadius), Vector3.up);
        GenerateBase(innerRadius, Vector3.zero, Vector3.down, true);
    }

    void GenerateCylinderVerticalCircle(float innerRadius, float outerRadius, Vector3 offset, Vector3 normal, bool inverted = false)
    {
        float stepX = (Mathf.PI * 2f) / quality;

        for(int x = 0; x < quality; x++)
        {
            Vector3 p1 = new Vector3(Mathf.Sin(stepX * x) * innerRadius, 0f, Mathf.Cos(stepX * x) * innerRadius) + offset;
            Vector3 p2 = new Vector3(Mathf.Sin(stepX * x) * outerRadius, 0f, Mathf.Cos(stepX * x) * outerRadius) + offset;
            Vector3 p3 = new Vector3(Mathf.Sin(stepX * (x + 1)) * innerRadius, 0f, Mathf.Cos(stepX * (x + 1)) * innerRadius) + offset;
            Vector3 p4 = new Vector3(Mathf.Sin(stepX * (x + 1)) * outerRadius, 0f, Mathf.Cos(stepX * (x + 1)) * outerRadius) + offset;

            AddTriangle(p1, p2, p3, normal, normal, normal, inverted);
            AddTriangle(p2, p4, p3, normal, normal, normal, inverted);
        }
    }

    void GenerateBase(float radius, Vector3 offset, Vector3 normal, bool inverted = false)
    {
        float stepX = (Mathf.PI * 2f) / quality;

        for (int x = 0; x < quality; x++)
        {
            Vector3 p1 = offset;
            Vector3 p2 = new Vector3(Mathf.Sin(stepX * x) * radius, 0f, Mathf.Cos(stepX * x) * radius) + offset;
            Vector3 p3 = new Vector3(Mathf.Sin(stepX * (x + 1)) * radius, 0f, Mathf.Cos(stepX * (x + 1)) * radius) + offset;

            AddTriangle(p1, p2, p3, normal, normal, normal, inverted);
        }
    }

    void GenerateCylinderSideCircle(float radius, Vector3 offset, bool inverted = false)
    {
        float stepY = height / segments;

        for(int y = 0; y < segments; y++)
        {
            float stepX = (Mathf.PI * 2f) / quality;

            for(int x = 0; x < quality; x++)
            {
                Vector3 p1 = new Vector3(Mathf.Sin(stepX * x) * radius, stepY * y, Mathf.Cos(stepX * x) * radius) + offset;
                Vector3 p2 = new Vector3(Mathf.Sin(stepX * (x + 1)) * radius, stepY * y, Mathf.Cos(stepX * (x + 1)) * radius) + offset;
                Vector3 p3 = new Vector3(Mathf.Sin(stepX * x) * radius, stepY * (y + 1), Mathf.Cos(stepX * x) * radius) + offset;
                Vector3 p4 = new Vector3(Mathf.Sin(stepX * (x + 1)) * radius, stepY * (y + 1), Mathf.Cos(stepX * (x + 1)) * radius) + offset;

                Vector3 n1 = new Vector3(Mathf.Sin(stepX * x) * radius, 0f, Mathf.Cos(stepX * x) * radius).normalized;
                Vector3 n2 = new Vector3(Mathf.Sin(stepX * (x + 1)) * radius, 0f, Mathf.Cos(stepX * (x + 1)) * radius).normalized;

                if(inverted)
                {
                    n1 = -n1;
                    n2 = -n2;
                }

                AddTriangle(p1, p2, p3, n1, n2, n1, inverted);
                AddTriangle(p2, p4, p3, n2, n2, n1, inverted);
            }
        }
    }

    void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 n1, Vector3 n2, Vector3 n3, bool inverted = false)
    {
        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);

        normals.Add(n1);
        normals.Add(n2);
        normals.Add(n3);

        int triangleCount = vertices.Count;
        triangles.Add(triangleCount - 1);

        if(!inverted)
        {
            triangles.Add(triangleCount - 3);
            triangles.Add(triangleCount - 2);
        }
        else
        {
            triangles.Add(triangleCount - 2);
            triangles.Add(triangleCount - 3);
        }
    }
}
