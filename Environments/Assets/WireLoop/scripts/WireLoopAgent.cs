using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WireLoopAgent : Agent {

    public CircularRayPerception circularRayPerception;
    public RigResetPosition[] rigs;

    public Rigidbody armSlider;
    public Rigidbody armBase;
    public Rigidbody armLow;
    public Rigidbody armHigh;
    public Rigidbody armTop;
    public Rigidbody armRotator;

    public Transform head;

    public float force;
    private readonly float[] _movementBuckets = new float[] { -1.0f, -0.8f, -0.6f, -0.4f, -0.2f, 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };

    private static string[] detectableObjects = {"Enemy"};

    public Transform ring;
    internal Vector3 ringOldPos;
    internal Vector3 ringNewPos;
    internal Vector3 ringVelocity;
    internal float localXVelocity;

    public float trackReward = 0f;

    public override void InitializeAgent()
    {
        for(int i = 0; i < rigs.Length; i++)
        {
            rigs[i].EvaluateStart();
            rigs[i].Reset();
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(circularRayPerception.Perceive(detectableObjects));
        AddVectorObs(armSlider.transform.localPosition.x);
        AddVectorObs(armBase.transform.localEulerAngles.z / 360f);
        AddVectorObs(armLow.transform.localEulerAngles.x / 360f);
        AddVectorObs(armHigh.transform.localEulerAngles.x / 360f);
        AddVectorObs(armTop.transform.localEulerAngles.x / 360f);
        AddVectorObs(armRotator.transform.localEulerAngles.y / 360f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Calculates Ring Velocity
        ringNewPos = ring.position;
        Vector3 media = ringNewPos - ringOldPos;
        ringVelocity = media / Time.fixedDeltaTime;
        ringOldPos = ringNewPos;

        // Adds reward depending on velocity
        // HACK Prevent velocity spikes on reset
        if (Mathf.Abs(localXVelocity) > 5f)
            Debug.LogWarning("Velocity exceeded clamp value");
        localXVelocity = Mathf.Clamp(ring.transform.InverseTransformVector(ringVelocity).x, -5f, 5f);

        AddReward(localXVelocity * 0.01f);
        trackReward += localXVelocity * 0.01f;

        armSlider.AddRelativeForce(new Vector3(1f, 0f, 0f) * _movementBuckets[(int)vectorAction[0]] * force, ForceMode.Impulse);
        armBase.AddRelativeTorque(new Vector3(0f, 0f, 4f) * _movementBuckets[(int)vectorAction[1]] * force, ForceMode.Impulse);
        armLow.AddRelativeTorque(new Vector3(3f, 0f, 0f) * _movementBuckets[(int)vectorAction[2]] * force, ForceMode.Impulse);
        armHigh.AddRelativeTorque(new Vector3(2f, 0f, 0f) * _movementBuckets[(int)vectorAction[3]] * force, ForceMode.Impulse);
        armTop.AddRelativeTorque(new Vector3(1f, 0f, 0f) * _movementBuckets[(int)vectorAction[4]] * force, ForceMode.Impulse);
        armRotator.AddRelativeTorque(new Vector3(0f, 1f, 0f) * _movementBuckets[(int)vectorAction[5]] * force, ForceMode.Impulse);
    }

    public override void AgentReset()
    {
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].Reset();
        }
    }

    public void Loose()
    {
        AddReward(-1f);
        Done();
    }
}
