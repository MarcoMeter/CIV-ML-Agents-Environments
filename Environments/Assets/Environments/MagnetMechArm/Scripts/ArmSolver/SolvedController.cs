using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolvedController : MonoBehaviour
{

    public Transform targetPoint;
    public Transform head;
    private Vector3 targetPosition = Vector3.zero;

    public float armLength = 3f;

    private float alpha = 0f;
    private float beta = 0f;
    private float gamma = 0f;

    private float baseAngle = 0f;
    private float angle1 = 0f;
    private float angle2 = 0f;

    public MechJoint baseJoint;
    public MechJoint joint1;
    public MechJoint joint2;
    public MechJoint joint3;
    public MechJoint joint4;

    [Range(0f, 1f)]
    public float inputRot = 0f;
    [Range(0f, 1f)]
    public float inputHeight = 0f;
    [Range(0.18f, 1f)]
    public float inputDist = 0f;

    // Start is called before the first frame update
    void Start()
    {
        armLength = (joint1.joint.position - joint2.joint.position).magnitude;
    }

    // Update is called once per frame
    public void NextFrame()
    {
        CalculateTargets();
        Step();
    }

    public void CalculateTargets()
    {
        float flatDist = inputDist * armLength * 2f;
        float dist = new Vector3(flatDist, inputHeight * armLength * 2f, 0f).magnitude + 0.00001f;

        alpha = Mathf.Atan((inputHeight * armLength * 2f) / flatDist);

        dist = Mathf.Clamp(dist, 0f, armLength * 2f);
        angle1 = Mathf.Acos((dist * 0.5f) / armLength);
        angle2 = Mathf.Asin((dist * 0.5f) / armLength);

        Vector3 headTarget = targetPoint.position - (head.position - joint3.joint.position) * 2f;

        baseAngle = Mathf.Atan(Mathf.Abs(headTarget.x) / Mathf.Abs(headTarget.z));
        if (headTarget.z < 0f)
        {
            baseAngle = Mathf.PI - baseAngle;
        }
        if (headTarget.x < 0f)
        {
            baseAngle = Mathf.PI * 2f - baseAngle;
        }

        alpha = alpha * (180f / Mathf.PI);
        angle1 = angle1 * (180f / Mathf.PI);
        angle2 = angle2 * (180f / Mathf.PI);
        baseAngle = baseAngle * (180f / Mathf.PI);

        float targetRot1 = 90f - angle1 - alpha;
        float targetRot2 = 180f - angle2 * 2f;

        baseJoint.targetRotation = Mathf.Clamp(inputRot * 2f - 1f, -1f, 1f);
        joint1.targetRotation = targetRot1;
        joint2.targetRotation = targetRot2;
        joint3.targetRotation = angle2 - 90f + alpha;
        joint4.targetRotation = -90f;
    }

    public void Step()
    {
        baseJoint.Step(Time.deltaTime * 100f);
        joint1.Step(Time.deltaTime * 100f);
        joint2.Step(Time.deltaTime * 100f);
        joint3.Step(Time.deltaTime * 100f);
        joint4.Step(Time.deltaTime * 100f);
    }

    public void SetPositions()
    {
        baseJoint.Set();
        joint1.Set();
        joint2.Set();
        joint3.Set();
        joint4.Set();
    }

    Vector3 RotateVector(Vector3 pos, float angle)
    {
        pos.x = Mathf.Cos(angle) * pos.x - Mathf.Sin(angle) * pos.y;
        pos.y = Mathf.Sin(angle) * pos.x + Mathf.Cos(angle) * pos.y;
        return pos;
    }

    public void ResetPositions()
    {
        setDist(0.5f);
        setHeight(0.5f);
        setRot(0.5f);
        SetPositions();
    }

    public void addDist(float val)
    {
        setDist(inputDist + val);
    }

    public void addHeight(float val)
    {
        setHeight(inputHeight + val);
    }

    public void addRot(float val)
    {
       setRot(inputRot + val);
    }

    public void setDist(float val)
    {
        inputDist = clampDist(val);
    }

    public void setHeight(float val)
    {
        inputHeight = clampHeight(val);
    }

    public void setRot(float val)
    {
        inputRot = clampRot(val);
    }

    public float clampHeight(float val)
    {
        return Mathf.Clamp(val, 0f, 1f);
    }

    public float clampDist(float val)
    {
        return Mathf.Clamp(val, 0.18f, 1f);
    }

    public float clampRot(float val)
    {
        return Mathf.Clamp(val, 0f, 1f);
    }
}
