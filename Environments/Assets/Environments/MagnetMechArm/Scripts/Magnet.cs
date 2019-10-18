using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Magnet : MonoBehaviour
{

    public bool hasMagnet = false;
    public Agent agent;
    public GenerateTestField generateObject;

    public void DropMagnet()
    {
        if(hasMagnet)
        {
            agent.AddReward(-0.1f);
            agent.Done();
        }
        else
        {
            hasMagnet = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
            //GetComponent<MeshRenderer>().enabled = hasMagnet;
            agent.AddReward(1f);
            generateObject.PlaceMagnet();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
