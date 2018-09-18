using MLAgents;
using UnityEngine;

public class CatcherAgent : Agent
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
        Vector3 relativePosition = TagSettings.instance.runner.transform.position - transform.position;

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
        float distanceToRunner = Vector3.Distance(
            transform.position,
            TagSettings.instance.runner.transform.position);

        // Caught runner
        if (distanceToRunner < 1.1f)
        {
            AddReward(1f);
            TagSettings.instance.runner.GotCaught();
        }

        // Getting closer
        if (distanceToRunner < previousDistance) AddReward(0.01f);

        // Fell off platform
        if (transform.position.y < 0f)
        {
            AddReward(-1.0f);
            Done();
        }

        previousDistance = distanceToRunner;

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
}
