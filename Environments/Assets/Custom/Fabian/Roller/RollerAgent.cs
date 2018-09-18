using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    public float speed = 10f;
    private float previousDistance = float.MaxValue;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Transform target;

    public override void AgentReset()
    {
        if (transform.position.y < -1.0f)
        {
            transform.position = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        else
        {
            target.position = new Vector3(
                Random.value * 8 - 4,
                0.5f,
                Random.value * 8 - 4);
        }
    }

    public override void CollectObservations()
    {
        Vector3 relativePosition = target.position - transform.position;

        AddVectorObs(relativePosition.x / 5);
        AddVectorObs(relativePosition.z / 5);

        AddVectorObs((transform.position.x + 5) / 5);
        AddVectorObs((transform.position.x - 5) / 5);
        AddVectorObs((transform.position.z + 5) / 5);
        AddVectorObs((transform.position.z - 5) / 5);

        AddVectorObs(rb.velocity.x / 5);
        AddVectorObs(rb.velocity.z / 5);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float distanceToTarget = Vector3.Distance(
              transform.position,
              target.position);

        // Reaching the target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Getting closer
        if (distanceToTarget < previousDistance) AddReward(0.1f);

        // Time penalty
        AddReward(-0.05f);

        // Fell off platform
        if(transform.position.y < -1.0f)
        {
            AddReward(-1.0f);
            Done();
        }

        previousDistance = distanceToTarget;

        // Actions size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rb.AddForce(controlSignal * speed);
    }
}
