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
        AddReward(-0.001f);

        robot.inputBase = vectorAction[0];
        robot.inputArm1 = vectorAction[1];
        robot.inputArm2 = vectorAction[2];
        robot.inputRing = vectorAction[3];
    }

    public override void AgentOnDone()
    {
        base.AgentOnDone();
    }

    public override void AgentReset()
    {
        robot.armBase = initialRobotPosition.armBase;
        robot.arm1 = initialRobotPosition.arm1;
        robot.arm2 = initialRobotPosition.arm2;
        robot.ring = initialRobotPosition.ring;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            ring.position,
            ring.position + ring.transform.right * localXVelocity);

        Handles.Label(ring.position, ring.position + "\n" + localXVelocity.ToString());
    }
}
