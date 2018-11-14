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
    private readonly float[] _movementBuckets = new float[] { -1.0f, -0.1f, 0f, 0.1f, 1.0f };

    private static string[] detectableObjects = {"Enemy"};

    public Transform ring;
    internal Vector3 ringOldPos;
    internal Vector3 ringNewPos;
    internal Vector3 ringVelocity;
    internal float localXVelocity;

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

        //velocitys
        AddVectorObs(transform.InverseTransformDirection(armSlider.velocity).x);
        AddVectorObs(armBase.angularVelocity);
        AddVectorObs(armLow.angularVelocity);
        AddVectorObs(armHigh.angularVelocity);
        AddVectorObs(armTop.angularVelocity);
        AddVectorObs(armRotator.angularVelocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Calculates Ring Velocity
        ringNewPos = ring.position;
        Vector3 media = ringNewPos - ringOldPos;
        ringVelocity = media / Time.fixedDeltaTime;
        ringOldPos = ringNewPos;

        localXVelocity = Mathf.Clamp(ring.transform.InverseTransformVector(ringVelocity).x, -5f, 5f);
        AddReward(localXVelocity * 0.01f);
        AddReward(-0.001f);

        armSlider.AddRelativeForce(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[0]] * force, ForceMode.Impulse);
        armBase.AddRelativeTorque(new Vector3(0f, 0f, 10f) * _movementBuckets[(int)vectorAction[1]] * force, ForceMode.Impulse);
        armLow.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[2]] * force, ForceMode.Impulse);
        armHigh.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[3]] * force, ForceMode.Impulse);
        armTop.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[4]] * force, ForceMode.Impulse);
        armRotator.AddRelativeTorque(new Vector3(0f, 10f, 0f) * _movementBuckets[(int)vectorAction[5]] * force, ForceMode.Impulse);
    }

    public override void AgentReset()
    {
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].Reset();
        }
        ringNewPos = ring.position;
        ringOldPos = ring.position;
        ringVelocity = Vector3.zero;
    }

    public void Loose()
    {
        AddReward(-1f);
        Done();
    }
}
