using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AssemblyAgent : Agent {

    public Rigidbody mover;
    private Vector3 moverOrigin;
    public Vector3 maxMoverDirection = new Vector3(0f, 0f, 1f);
    RayPerception rayPer;
    public SpawnCubes spawner;

    public Transform raySpawner;
    public float rayDistance = 0.1f;
    public Vector2 rayAmount = new Vector2(5, 5);

    public void Start()
    {

    }

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
        moverOrigin = mover.transform.position;
    }

    public override void CollectObservations()
    {
        /*float rayDistance = 12f;

        

        string[] detectableObjects = { "blueBox", "redBox", "Untagged" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(transform.localEulerAngles.y / 360f);*/

        Ray[] rays = new Ray[(int)rayAmount.x * (int)rayAmount.y];

        float halfX = (((int)rayAmount.x - 1) * rayDistance) / 2.0f;
        float halfY = (((int)rayAmount.y - 1) * rayDistance) / 2.0f;

        string[] detectableObjects = { "blueBox", "redBox", "Untagged" };

        Matrix4x4 rot = Matrix4x4.Rotate(raySpawner.rotation);
        for (int x = 0; x < (int)rayAmount.x; x++)
        {
            for (int y = 0; y < (int)rayAmount.y; y++)
            {
                Vector3 origin = new Vector3(x * rayDistance - halfX, y * rayDistance - halfY, 0f);
                Vector3 direction = new Vector3(0, 0, 1f);
                origin = rot.MultiplyPoint3x4(origin);
                direction = rot.MultiplyPoint3x4(direction);
                origin += raySpawner.position;
                Ray ray = new Ray(origin, direction);
                rays[(x * (int)rayAmount.x) + y] = ray;
            }
        }

        AddVectorObs(rayPer.Perceive(12f, rays, detectableObjects, 0f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float actionY = Mathf.Clamp(vectorAction[0], -1f, 1f);
        mover.MovePosition(Vector3.Lerp(mover.position, moverOrigin + maxMoverDirection * actionY, Time.deltaTime * 10.0f));

        //AddReward(0.001f);
    }

    public override void AgentReset()
    {
        spawner.ResetCubes();
        mover.position = moverOrigin;
    }
}
