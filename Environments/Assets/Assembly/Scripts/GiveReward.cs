using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class GiveReward : MonoBehaviour {

    public float reward = 1f;
    public float punish = 0f;
    public string acceptedClass = "blueClass";
    public Agent agent;

	// Update is called once per frame
	void OnCollisionEnter (Collision collision) {
        if(collision.transform.gameObject.tag == acceptedClass)
        {
            agent.AddReward(reward);
        }
        else
        {
            agent.AddReward(-punish);
        }
        Destroy(collision.transform.gameObject);
	}
}
