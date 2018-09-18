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
        if(collision.transform.tag == "AssemblyLine")
        {
            rig.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
        }
    }
}
