using System;
using MLAgents;
using UnityEditor;
using UnityEngine;

public class RobotAgent : Agent
{
    public CircularRayPerception circularRayPerception;
    public string[] detectabeObjects;
    public Rigidbody ringRB;

    private RobotController robot;
    private RobotTelemetryData initialRobotPosition;

    public override void InitializeAgent()
    {
        robot = GetComponent<RobotController>();
        initialRobotPosition = new RobotTelemetryData(robot);
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
        AddReward(ringRB.velocity.x * 0.01f);
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
        Gizmos.DrawLine(ringRB.position, ringRB.position + ringRB.velocity);
        Handles.Label(ringRB.position, ringRB.position + "\n" + ringRB.velocity.ToString());
    }
}
