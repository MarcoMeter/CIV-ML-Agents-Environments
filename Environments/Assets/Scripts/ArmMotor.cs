using System;
using UnityEngine;

public class ArmMotor : MonoBehaviour
{
    public new Rigidbody rigidbody;

    public float rotationSpeed;
    public float movementSpeed;

    public Vector3 MoveInput { get; set; }


    private void FixedUpdate()
    {
        Move();
        //Rotate();
    }

    /// <summary>
    /// Moves arm on X/Z axis
    /// </summary>
    private void Move()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.MovePosition(
            Vector3.ClampMagnitude(
                rigidbody.position + (MoveInput * movementSpeed * Time.fixedDeltaTime), 
                LidAcademy.Instance.range));
    }
}
