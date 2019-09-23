using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MechJoint
{
    public Transform joint;
    private Rigidbody rig;
    public Vector3 axis = Vector3.up;
    public Vector3 initialRotation = Vector3.zero;
    public float targetRotation = 0f;
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public bool targetAngle = false;
    public void Init()
    {
        if(!(rig = joint.GetComponent<Rigidbody>()))
        {
            rig = joint.gameObject.AddComponent<Rigidbody>();
            rig.isKinematic = true;
            rig.useGravity = false;
        }
    }

    public void AddTargetRotation(float rot)
    {
        targetRotation = Mathf.Clamp(targetRotation + rot, -1f, 1f);
    }

    public void Step(float speed)
    {
        if(targetAngle)
        {
            joint.transform.localRotation = Quaternion.Lerp(joint.transform.localRotation, Quaternion.Euler(targetRotation * axis.x, targetRotation * axis.y, targetRotation * axis.z), speed * Time.deltaTime);
            return;
        }

        float lerpedInput = Mathf.Lerp(minAngle, maxAngle, (targetRotation + 1f) / 2f);
        joint.transform.localRotation = Quaternion.Lerp(joint.transform.localRotation, Quaternion.Euler(lerpedInput * axis.x, lerpedInput * axis.y, lerpedInput * axis.z), speed * Time.deltaTime);
    }

    public void Set()
    {
        if (targetAngle)
        {
            joint.transform.localRotation = Quaternion.Euler(targetRotation * axis.x, targetRotation * axis.y, targetRotation * axis.z);
            return;
        }

        float lerpedInput = Mathf.Lerp(minAngle, maxAngle, (targetRotation + 1f) / 2f);
        joint.transform.localRotation = Quaternion.Euler(lerpedInput * axis.x, lerpedInput * axis.y, lerpedInput * axis.z);
    }
}