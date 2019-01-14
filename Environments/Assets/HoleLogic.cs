using UnityEngine;

public class HoleLogic : MonoBehaviour
{
    public LayerMask lidLayer;
    public Collider attachCollider;

    private Collider storedLid;
    private float angularSpeedThresholdToAttach;

    private void FixedUpdate()
    {
        if (storedLid == null)
            return;

        Rigidbody lidRigidbody = storedLid.GetComponentInParent<Rigidbody>();

        if(lidRigidbody.angularVelocity.y > angularSpeedThresholdToAttach)
        {
            FixedJoint joint = storedLid.GetComponentInParent<FixedJoint>();
            joint.connectedBody = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (storedLid != null)
            return;

        if (1 << other.gameObject.layer != lidLayer)
            return;

        storedLid = other;

        Debug.Log($"Found {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (storedLid == null)
            return;

        if (other.gameObject != storedLid)
            return;

        storedLid = null;

        Debug.Log($"Lost {other.name}");
    }
}