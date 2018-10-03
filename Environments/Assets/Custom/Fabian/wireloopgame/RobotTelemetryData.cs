using UnityEngine;

internal class RobotTelemetryData
{
    private RobotController robot;
    internal Vector3 armBase;
    internal Vector3 armBaseRotator;
    internal Vector3 arm1, arm2;
    internal Vector3 ring;

    public RobotTelemetryData(RobotController robot)
    {
        this.robot = robot;

        armBase = robot.armBase;
        armBaseRotator = robot.armBaseRotator;
        arm1 = robot.arm1;
        arm2 = robot.arm2;
        ring = robot.ring;
    }
}