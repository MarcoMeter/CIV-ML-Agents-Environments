using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ArmController : MonoBehaviour
{

    public MechJoint[] joints = new MechJoint[0];
    public float speed = 5f;

    void Start()
    {
        foreach (MechJoint joint in joints)
        {
            joint.Init();
        }
    }

    public void Step()
    {
        foreach(MechJoint joint in joints)
        {
            joint.Step(speed);
        }
    }

    public void ResetPositions()
    {
        foreach (MechJoint joint in joints)
        {
            joint.joint.localEulerAngles = joint.initialRotation;
            joint.targetRotation = 0f;
        }
    }
}
