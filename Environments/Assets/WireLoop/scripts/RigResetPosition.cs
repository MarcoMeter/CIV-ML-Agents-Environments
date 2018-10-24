using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigResetPosition : MonoBehaviour {
    public Vector3 position;
    public Vector3 rotation;
    public Rigidbody rig;

    public bool startPosition = false;
    public bool startRotation = false;

    public void EvaluateStart()
    {
        if (startPosition)
        {
            position = rig.transform.localPosition;
        }

        if (startRotation)
        {
            rotation = rig.transform.localEulerAngles;
        }
    }

    public void Reset()
    {
        rig.isKinematic = true;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        rig.transform.localPosition = position;
        rig.transform.localEulerAngles = rotation;
        rig.isKinematic = false;
    }
}
