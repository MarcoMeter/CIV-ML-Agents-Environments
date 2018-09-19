using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Vector3 armBase;
    public Vector3 arm1, arm2;
    public Vector3 ring;

    public Transform armBaseTransform;
    public Transform arm1Transform, arm2Transform;
    public Transform ringTransform;

    public float inputBase, inputArm1, inputArm2, inputRing;
    public float speed;


    private void FixedUpdate()
    {
        ApplyInput();
        Clamps();
        Assignment();
    }

    private void ApplyInput()
    {
        armBase.z += Mathf.Clamp(inputBase, -1f, 1f) * speed * Time.fixedDeltaTime;
        arm1.z += Mathf.Clamp(inputArm1, -1f, 1f) * speed * Time.fixedDeltaTime;
        arm2.z += Mathf.Clamp(inputArm2, -1f, 1f) * speed * Time.fixedDeltaTime;
        ring.z += Mathf.Clamp(inputRing, -1f, 1f) * speed * 10f * Time.fixedDeltaTime;
    }

    private void Clamps()
    {
        armBase.z = Mathf.Clamp(armBase.z, armBase.x, armBase.y);
        arm1.z = Mathf.Clamp(arm1.z, arm1.x, arm1.y);
        arm2.z = Mathf.Clamp(arm2.z, arm2.x, arm2.y);
        ring.z = Mathf.Clamp(ring.z, ring.x, ring.y);
    }

    private void Assignment()
    {
        armBaseTransform.localEulerAngles = new Vector3(
            0f,
            0f,
            armBase.z);

        arm1Transform.localEulerAngles = new Vector3(
            arm1.z,
            0f,
            0f);

        arm2Transform.localEulerAngles = new Vector3(
            arm2.z,
            0f,
            0f);

        ringTransform.localEulerAngles = new Vector3(
            0f,
            ring.z,
            0f);
    }

    public List<float> GetTelemetry()
    {
        List<float> result = new List<float>
        {
            NormalizeValue(armBase.z, armBase.x, armBase.y),
            NormalizeValue(arm1.z, arm1.x, arm1.y),
            NormalizeValue(arm2.z, arm2.x, arm2.y),
            NormalizeValue(ring.z, ring.x, ring.y)
    };

        return result;
    }

    public float NormalizeValue(float value, float min, float max)
    {
        //Calculate the normalized float;
        float normalizedFloat = (value - min) / (max - min);
        //Clamp the "i" float between "min" value and "max" value
        value = Mathf.Clamp(value, min, max);
        //Clamp the normalized float between 0 and 1
        normalizedFloat = Mathf.Clamp(normalizedFloat, 0, 1);

        return normalizedFloat;
    }
}
