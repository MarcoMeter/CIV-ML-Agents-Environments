using System.Collections;
using UnityEngine;

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

        storedLid = other.gameObject;

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

    public void Attach(bool state)
    {
        if (storedLid == null)
            return;

        if (state == false)
        {
            storedLid.transform.position = attachCollider.transform.position;
            storedLid.transform.rotation = attachCollider.transform.rotation;

            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = storedLid.transform.GetComponentInParent<Rigidbody>();
            fixedJoint.connectedBody.useGravity = false;
        }
        else
        {
            fixedJoint.connectedBody.useGravity = true;
            StartCoroutine(WaitOneFixedTimeStepWakeup(fixedJoint.connectedBody));
            Destroy(fixedJoint);
        }
    }

    private IEnumerator WaitOneFixedTimeStepWakeup(Rigidbody rigidbody)
    {
        yield return new WaitForFixedUpdate();

        rigidbody.WakeUp();
    }
}
