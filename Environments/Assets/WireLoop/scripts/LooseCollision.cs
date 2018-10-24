using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseCollision : MonoBehaviour {

    public WireLoopAgent wireloopAgent;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            wireloopAgent.Loose();
        }
    }
}
