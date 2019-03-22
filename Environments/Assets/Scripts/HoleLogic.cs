using UnityEngine;

/*
 * BROKEN: Similar to HookLogic, not fully functional yet
 * Needs refractoring compared to HookLogic.cs
 * Joint logic should be moved in new script attached to lid itself
 */

public class HoleLogic : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Start() => initialPosition = transform.position;

    public void MoveToRandomPosition(Vector3 range) => transform.position = initialPosition.RandomDisplace2D(range);
    public void MoveToRandomPosition(float range) => transform.position = initialPosition.RandomDisplace2D(range);

    //private void FixedUpdate()
    //{
    //    if (storedLid == null)
    //        return;

    //    Rigidbody lidRigidbody = storedLid.GetComponentInParent<Rigidbody>();

    //    if(lidRigidbody.angularVelocity.y > angularSpeedThresholdToAttach)
    //    {
    //        FixedJoint joint = storedLid.GetComponentInParent<FixedJoint>();
    //        joint.connectedBody = null;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (storedLid != null)
    //        return;

    //    if (1 << other.gameObject.layer != lidLayer)
    //        return;

    //    storedLid = other;

    //    Debug.Log($"Found {other.name}");
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (storedLid == null)
    //        return;

    //    if (other.gameObject != storedLid)
    //        return;

    //    storedLid = null;

    //    Debug.Log($"Lost {other.name}");
    //}
}