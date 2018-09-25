using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLineMover : MonoBehaviour {
    Rigidbody rig;

    ConstantForce cf;

    public float speed = 30f;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
    }

    private void OnCollisionStay(Collision collision)
    {
        collision.rigidbody.MovePosition(collision.transform.position + Vector3.right * speed * Time.deltaTime);
        //collision.rigidbody.velocity = new Vector3(speed, collision.rigidbody.velocity.y, collision.rigidbody.velocity.z);
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.rigidbody.AddForce(Vector3.right * speed * collision.rigidbody.mass, ForceMode.Impulse);
    }
}
