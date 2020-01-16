using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SolvedMechArmAgent : Agent
{
    public SolvedController armController;
    public Transform head;
    public Transform spotLight;

    public bool grabbing = false;

    public GenerateTestField generator;

    public int tries = 20;

    public override void AgentReset()
    {
        Debug.Log("Resetting Agent");
        armController.ResetPositions();
        generator.Reset();
        /*if(tries > 20 || tries < 0)
        {
            generator.Reset();
            tries = 0;
        }
        tries++;*/

        // Reset Light Position
        spotLight.position = new Vector3(
            transform.position.x + Random.Range(-5f, 5f),
            transform.position.y + Random.Range(20f, 40f),
            transform.position.z + Random.Range(-5f, 5f)
        );
        spotLight.rotation = Quaternion.Euler(
            Random.Range(80f, 100f),
            Random.Range(0f, 360f),
            0f
        );

        grabbing = true;
    }

    public override void CollectObservations()
    {
        AddVectorObs(armController.inputDist);
        AddVectorObs(armController.inputRot);
        AddVectorObs(armController.inputHeight);
        AddVectorObs(grabbing);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Rewards
        float[] actions = new float[5] { -0.1f, -0.01f, 0f, 0.01f, 0.1f };

        int i = 0;

        grabbing = (int)vectorAction[i++] == 0 ? false : true;
        armController.addHeight(actions[(int)vectorAction[i++]]);
        armController.addDist(actions[(int)vectorAction[i++]]);
        armController.addRot(actions[(int)vectorAction[i++]]);
        armController.NextFrame();

        AddReward(-0.00001f);

        if (!grabbing)
        {
            RaycastHit hit;
            if (Physics.Raycast(head.position, head.forward, out hit, 3f))
            {
                Debug.DrawRay(head.position, head.forward * hit.distance, Color.red);
                Magnet magnet = null;
                if (magnet = hit.transform.GetComponent<Magnet>())
                {
                    magnet.DropMagnet();
                }
                else
                {
                    AddReward(-0.001f);
                    Done();
                }
            }
            else
            {
                //AddReward(-1f);
                //Done();
            }
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[4];

        action[0] = 2f;
        action[1] = 2f;
        action[2] = 2f;
        action[3] = 2f;
        return action;
    }
}
