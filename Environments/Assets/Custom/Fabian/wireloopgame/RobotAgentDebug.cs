using UnityEngine;

public class RobotAgentDebug : MonoBehaviour
{
    public float reward = 0f;
    private Vector3 ringNewPos;
    private float localXVelocity;
    private Vector3 ringVelocity;
    private Vector3 ringOldPos;

    private void Start()
    {
        ringOldPos = transform.position;
    }

    private void FixedUpdate()
    {
        ringNewPos = transform.position;
        var media = (ringNewPos - ringOldPos);
        ringVelocity = media / Time.fixedDeltaTime;
        ringOldPos = ringNewPos;
        ringNewPos = transform.position;

        localXVelocity = transform.transform.InverseTransformVector(ringVelocity).x;
        reward += localXVelocity * 0.01f;
        //reward += -0.0001f;
    }
}