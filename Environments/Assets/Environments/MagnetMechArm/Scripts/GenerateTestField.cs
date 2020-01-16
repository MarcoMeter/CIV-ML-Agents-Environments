using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class GenerateTestField : MonoBehaviour
{
    public Academy academy;
    public Vector3 minSize = new Vector3(3f, 0.5f, 3f);
    public Vector3 maxSize = new Vector3(5f, 1f, 5f);
    public Vector2 partsRange = new Vector2(5, 7);
    public float magnetChance = 0.1f;

    private float width;
    private float depth;
    private int partsX;
    private int partsZ;

    public Agent agent;

    public int partCount = 0;
    public int magnets = 0;
    public int placedMagnets = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateObject();
    }

    void Randomize()
    {
        width = Random.Range(minSize.x, maxSize.x);
        depth = Random.Range(minSize.z, maxSize.z);
        partsX = Mathf.FloorToInt(Random.Range(partsRange.x, partsRange.y));
        partsZ = Mathf.FloorToInt(Random.Range(partsRange.x, partsRange.y));
        partCount = partsX * partsZ;
    }

    public void Reset()
    {
        transform.rotation = Quaternion.identity;
        magnetChance = 0.8f;
        /*Random.InitState((int)Academy.Instance.resetParameters["seed"]);
        magnetChance = academy.resetParameters["magnet-chance"];
        academy.resetParameters["magnet-chance"] = academy.resetParameters["magnet-chance"] - academy.resetParameters["magnet-chance-loss"];
        academy.resetParameters["magnet-chance"] = academy.resetParameters["magnet-chance"] < academy.resetParameters["magnet-chance-min"] ? 
            academy.resetParameters["magnet-chance-min"] : 
            academy.resetParameters["magnet-chance"];*/
        partCount = 0;
        magnets = 0;
        placedMagnets = 0;
        DeleteChildren();
        GenerateObject();
        transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);
    }

    public void DeleteChildren()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }

    public void PlaceMagnet()
    {
        placedMagnets += 1;
        if(placedMagnets >= magnets)
        {
            agent.AddReward(1f);
            agent.Done();
        }
    }

    void GenerateObject()
    {
        Randomize();
        float partWidth = width / partsX;
        float partDepth = depth / partsZ;
        magnets = 0;
        for (int x = 0; x < partsX; x++)
        {
            for(int z = 0; z < partsZ; z++)
            {
                float height = Random.Range(minSize.y, maxSize.y);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if(Random.value < magnetChance)
                {
                    cube.AddComponent<Magnet>();
                    Magnet magnet = cube.GetComponent<Magnet>();
                    magnet.agent = agent;
                    cube.GetComponent<MeshRenderer>().material.color = Color.black;
                    magnets += 1;
                    magnet.generateObject = this;
                }
                Vector3 position = new Vector3(partWidth * x, height / 2f, partDepth * z);
                Vector3 scale = new Vector3(partWidth, height, partDepth);
                cube.transform.localScale = scale;
                cube.transform.parent = transform;
                cube.transform.localPosition = position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
