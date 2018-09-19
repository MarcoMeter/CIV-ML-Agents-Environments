using UnityEngine;

public class RobotController : MonoBehaviour
{   
    public Vector3 armBase;
    public Vector3 arm1, arm2;
    public Vector3 ring;

    public Transform armBaseTransform;
    public Transform arm1Transform, arm2Transform;
    public Transform ringTransform;
    
    private void FixedUpdate()
    {
        armBase.z = Mathf.Clamp(armBase.z, armBase.x, armBase.y);
        arm1.z = Mathf.Clamp(arm1.z, arm1.x, arm1.y);
        arm2.z = Mathf.Clamp(arm2.z, arm2.x, arm2.y);
        ring.z = Mathf.Clamp(ring.z, ring.x, ring.y);

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
}