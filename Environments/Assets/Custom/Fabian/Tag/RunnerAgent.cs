using MLAgents;
using UnityEngine;

public class RunnerAgent : Agent
{
    [SerializeField] private float speed;

    private Rigidbody rb;
    private float previousDistance;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void InitializeAgent()
    {
    }

    public override void CollectObservations()
    {
        Vector3 relativePosition = TagSettings.instance.catcher.transform.position - transform.position;

        AddVectorObs(relativePosition.x / 10f);
        AddVectorObs(relativePosition.z / 10f);

        AddVectorObs((transform.position.x + 10f) / 10f);
        AddVectorObs((transform.position.x - 10f) / 10f);
        AddVectorObs((transform.position.z + 10f) / 10f);
        AddVectorObs((transform.position.z - 10f) / 10f);

        AddVectorObs(rb.velocity.x / 10f);
        AddVectorObs(rb.velocity.z / 10f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float distanceToCatcher = Vector3.Distance(
            transform.position,
            TagSettings.instance.catcher.transform.position);

        // Getting further away
        if (distanceToCatcher > previousDistance) AddReward(0.01f);

        // Fell off platform
        if (transform.position.y < 0f)
        {
            AddReward(-1.0f);
            Done();
        }

        previousDistance = distanceToCatcher;

        // Actions size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rb.AddForce(controlSignal * speed);
    }

    public override void AgentOnDone()
    {
    }

    public override void AgentReset()
    {
        rb.velocity = Vector3.zero;
        rb.MovePosition(
            new Vector3(
                Random.Range(-8f, 8f),
                TagSettings.instance.spawn.position.y,
                Random.Range(-8f, 8f)));
    }

    public void GotCaught()
    {
        AddReward(-1.0f);
        Done();
    }
}
