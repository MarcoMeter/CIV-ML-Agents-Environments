using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Transforms")]
    public Transform armBaseTransform;
    public Transform armBaseRotatorTransform;
    public Transform arm1Transform, arm2Transform;
    public Transform ringRotatorTransform;
    public Transform ringTransform;

    [Header("Axle Settings")]
    public Vector3 armBase;
    public Vector3 armBaseRotator;
    public Vector3 arm1, arm2;
    public Vector3 ring;

    [Header("Settings")]
    public float speedBase;
    public float speedArms;
    public float speedRing;

    [Header("Debug")]
    public float inputBase;
    public float inputBaseRotator;
    public float inputArm1;
    public float inputArm2;
    public float inputRing;


    private void FixedUpdate()
    {
        ApplyInput();
        Clamps();
        Assignment();
    }

    private void ApplyInput()
    {
        armBase.z += inputBase * speedBase * Time.fixedDeltaTime;
        armBaseRotator.z += inputBaseRotator * speedArms * Time.fixedDeltaTime;
        arm1.z += inputArm1 * speedArms * Time.fixedDeltaTime;
        arm2.z += inputArm2 * speedArms * Time.fixedDeltaTime;
        ring.z += inputRing * speedRing * Time.fixedDeltaTime;
    }

    private void Clamps()
    {
        armBase.z = Mathf.Clamp(armBase.z, armBase.x, armBase.y);
        armBaseRotator.z = Mathf.Clamp(armBaseRotator.z, armBaseRotator.x, armBaseRotator.y);
        arm1.z = Mathf.Clamp(arm1.z, arm1.x, arm1.y);
        arm2.z = Mathf.Clamp(arm2.z, arm2.x, arm2.y);
        ring.z = Mathf.Clamp(ring.z, ring.x, ring.y);
    }

    private void Assignment()
    {
        armBaseTransform.localPosition = new Vector3(
            armBase.z,
            armBaseTransform.localPosition.y,
            armBaseTransform.localPosition.z);

        armBaseRotatorTransform.localEulerAngles = new Vector3(
            0f,
            0f,
            armBaseRotator.z);

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
        var result = new List<float>
        {
            NormalizeValue(armBase.z, armBase.x, armBase.y),
            NormalizeValue(armBaseRotator.z, armBaseRotator.x, armBaseRotator.y),
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
