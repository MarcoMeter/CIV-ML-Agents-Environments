using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AssemblyAgent : Agent {

    public Transform gun;
    RayPerception rayPer;

    public override void InitializeAgent()
    {
        rayPer = gun.GetComponent<RayPerception>();
    }

    public override void CollectObservations()
    {
        float rayDistance = 12f;
        float[] rayAngles = { 60f, 70f, 80f, 85f, 88f, 90f, 92f, 95f, 100f, 110f, 120f };
        string[] detectableObjects = { "blueBox", "redBox", "Untagged" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float actionY = Mathf.Clamp(vectorAction[0], -1f, 1f);
        gun.Rotate(new Vector3(0, 2f, 0), actionY);

        float actionFire = Mathf.Abs(vectorAction[1]);

        
        RaycastHit hit;
        Ray ray = new Ray(gun.position, gun.forward);
        if (Physics.Raycast(ray, out hit, 12f))
        {
            if(actionFire > 0.5f)
            {
                Debug.DrawLine(gun.position, hit.point, Color.red);
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForce(gun.forward + Vector3.up * hit.rigidbody.mass, ForceMode.Impulse);
                }
            }
        }
    }

    public override void AgentReset()
    {
        gun.rotation = Quaternion.identity;
    }
}
