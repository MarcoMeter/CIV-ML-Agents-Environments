using System;
using MLAgents;
using UnityEditor;
using UnityEngine;

public class RobotAgent : Agent
{
    public CircularRayPerception circularRayPerception;
    public string[] detectabeObjects;
    public Transform ring;

    private RobotController robot;
    private RobotTelemetryData initialRobotPosition;
    private Vector3 ringOldPos;
    private float localXVelocity;
    private Vector3 ringNewPos;
    private Vector3 ringVelocity;

    private float[] _movementBuckets = new float[] { -1.0f, -0.8f, -0.6f, -0.4f, -0.2f, 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };


    public override void InitializeAgent()
    {
        robot = GetComponent<RobotController>();
        initialRobotPosition = new RobotTelemetryData(robot);
        ringOldPos = ring.position;
    }

    public override void CollectObservations()
    {
        AddVectorObs(circularRayPerception.Percieve(detectabeObjects));
        AddVectorObs(robot.GetTelemetry());
    }

    internal void Collision(bool good)
    {
        print("RobotAgent");

        if (good)
            AddReward(1.0f);
        else
            AddReward(-1.0f);

        Done();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        ringNewPos = ring.position;
        var media = (ringNewPos - ringOldPos);
        ringVelocity = media / Time.fixedDeltaTime;
        ringOldPos = ringNewPos;
        ringNewPos = ring.position;

        localXVelocity = ring.transform.InverseTransformVector(ringVelocity).x;
        AddReward(localXVelocity * 0.01f);
        AddReward(-0.0001f);

        if (brain.brainType == BrainType.Player)
        {
            robot.inputBase = (Input.GetKey(KeyCode.KeypadPeriod) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad0) ? -1f : 0f);
            robot.inputBaseRotator = (Input.GetKey(KeyCode.Keypad3) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad2) ? -1f : 0f);
            robot.inputArm1 = (Input.GetKey(KeyCode.Keypad7) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad4) ? -1f : 0f);
            robot.inputArm2 = (Input.GetKey(KeyCode.Keypad8) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad5) ? -1f : 0f);
            robot.inputRing = (Input.GetKey(KeyCode.Keypad9) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad6) ? -1f : 0f);
        }
        else
        {
            robot.inputBase = _movementBuckets[(int)vectorAction[0]];
            robot.inputBaseRotator = _movementBuckets[(int)vectorAction[1]];
            robot.inputArm1 = _movementBuckets[(int)vectorAction[2]];
            robot.inputArm2 = _movementBuckets[(int)vectorAction[3]];
            robot.inputRing = _movementBuckets[(int)vectorAction[4]];
        }
    }

    public override void AgentOnDone()
    {
        base.AgentOnDone();
    }

    public override void AgentReset()
    {
        robot.armBase = initialRobotPosition.armBase;
        robot.armBaseRotator = initialRobotPosition.armBaseRotator;
        robot.arm1 = initialRobotPosition.arm1;
        robot.arm2 = initialRobotPosition.arm2;
        robot.ring = initialRobotPosition.ring;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            ring.position,
            ring.position + ring.transform.right * localXVelocity);

        Handles.Label(ring.position, ring.position + "\n" + localXVelocity.ToString());
    }
#endif
}
