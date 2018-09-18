using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveReward : MonoBehaviour {

    public float reward = 1f;
    public string acceptedClass = "blueClass";
    public AssemblyAgent agent;

	// Update is called once per frame
	void OnCollisionEnter (Collision collision) {
        if(collision.transform.tag == acceptedClass)
        {
            agent.AddReward(reward);
        }
        else
        {
            //agent.AddReward(-reward / 2.0f);
        }
        Destroy(collision.transform.gameObject);
	}
}
