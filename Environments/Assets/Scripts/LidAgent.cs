using MLAgents;
using UnityEngine;

public class LidAgent : Agent
{
    public ArmMotor motor;

    private Vector3 initialPosition;
    private GameObject storedLid;
    private float[] _movementBuckets = new float[]
        {
            -1.0f,
            -0.75f,
            -0.5f,
            -0.25f,
            0.0f,
            0.25f,
            0.5f,
            0.75f,
            1.0f
        };


    private void Start() => initialPosition = transform.position;

    private void FixedUpdate() =>
        // Time penalty
        AddReward(-0.001f);

    public override void CollectObservations()
    {
        AddVectorObs(transform.position);
        AddVectorObs(LidAcademy.Instance.hole.transform.position);
        AddVectorObs(Vector3.Distance(transform.position, LidAcademy.Instance.hole.transform.position));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        motor.MoveInput = brain.brainType == BrainType.Player
            ? new Vector3(
                vectorAction[0],
                vectorAction[1],
                vectorAction[2])
            : new Vector3(
                _movementBuckets[(int)vectorAction[0]],
                _movementBuckets[(int)vectorAction[1]],
                _movementBuckets[(int)vectorAction[2]]);

        if (vectorAction[3] == 1f) Probe();
    }

    private void Probe()
    {
        if (storedLid == null)
        {
            AddReward(-1f);
            Done();
        }
        else
        {
            AddReward(1f);
            Done();
        }
    }

    public override void AgentReset()
    {
        transform.position = initialPosition;
        LidAcademy.Instance.hole.MoveToRandomPosition(LidAcademy.Instance.range);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (storedLid != null)
            return;

        if (1 << other.gameObject.layer != LidAcademy.Instance.lidLayer)
            return;

        storedLid = other.gameObject;

        Debug.Log($"Found {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (storedLid == null)
            return;

        if (other.gameObject != storedLid)
            return;

        storedLid = null;

        Debug.Log($"Lost {other.name}");
    }
}