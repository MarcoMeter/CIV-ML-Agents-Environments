using System.Collections;
using UnityEngine;

/* TODO: Refractoring
 * Duplicate code with HoleLogic.cs
 * Joint logic should be moved in new script attached to lid itself
 */

public class HookLogic : MonoBehaviour
{
    public LayerMask lidLayer;
    public Collider attachCollider;
    public FixedJoint fixedJoint;

    private GameObject storedLid;


    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != lidLayer)
            return;

        // Temp for current lid within acceptable bounds
        storedLid = other.gameObject;

        Debug.Log($"Found {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (storedLid == null)
            return;

        if (other.gameObject != storedLid)
            return;

        // Clear temp
        storedLid = null;

        Debug.Log($"Lost {other.name}");
    }

    /// <summary>
    /// Attach/Detach Lid to hook
    /// </summary>
    /// <param name="state">Attach/Detach</param>
    public void Attach(bool state)
    {
        if (storedLid == null)
            return;

        if (state == false)
        {
            // Get Rigidbody and set new position
            Rigidbody lidRigidbody = storedLid.GetComponentInParent<Rigidbody>();

            // BROKEN: no effect
            lidRigidbody.position = attachCollider.transform.position;
            lidRigidbody.rotation = attachCollider.transform.rotation;

            // Attach FixedJoint
            fixedJoint = storedLid.transform.root.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = GetComponent<Rigidbody>();
            lidRigidbody.useGravity = false;
        }
        else
        {
            // Destroy joint
            Rigidbody lidRigidbody = fixedJoint.GetComponentInParent<Rigidbody>();
            lidRigidbody.useGravity = true;
            
            // Rigidbody needs Wakeup on next step
            StartCoroutine(WaitOneFixedTimeStepWakeup(lidRigidbody));
            Destroy(fixedJoint);
        }
    }

    /// <summary>
    /// Wakes up rigidbody on next physics step
    /// </summary>
    /// <param name="rigidbody">Rigidbody to wake up</param>
    /// <returns></returns>
    private IEnumerator WaitOneFixedTimeStepWakeup(Rigidbody rigidbody)
    {
        yield return new WaitForFixedUpdate();

        rigidbody.WakeUp();
    }
}
