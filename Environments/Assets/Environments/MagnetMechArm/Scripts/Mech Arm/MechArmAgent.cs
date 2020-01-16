using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MechArmAgent : Agent
{
    public ArmController armController;

    public Transform head;

    public bool grabbing = false;

    public GenerateObject generator;

    public override void AgentReset()
    {
        armController.ResetPositions();

        generator.Reset();
    }

    public override void CollectObservations()
    {
        foreach (MechJoint joint in armController.joints)
        {
            AddVectorObs(joint.targetRotation);
        }

        AddVectorObs(grabbing);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Rewards
        float[] actions = new float[5] { -0.1f, -0.01f, 0f, 0.01f, 0.1f };

        int i = 0;

        grabbing = (int)vectorAction[i++] == 0 ? false : true;

        foreach (MechJoint joint in armController.joints)
        {
            float targetRot = actions[(int)vectorAction[i++]];
            joint.AddTargetRotation(targetRot);
            AddReward(-Mathf.Abs(targetRot * 0.01f));
        }

        armController.Step();

        float dot = Vector3.Dot(-head.forward, Vector3.down);

        AddReward(dot / 1000f);

        if(dot < -0.6f)
        {
            AddReward(-1f);
            Done();
        }

        if(!grabbing)
        {
            RaycastHit hit;
            if(Physics.Raycast(head.position, -head.forward, out hit, 5f))
            {
                Magnet magnet = null;
                if (magnet = hit.transform.GetComponent<Magnet>())
                {
                    magnet.DropMagnet();
                }
            }
        }
    }
}
