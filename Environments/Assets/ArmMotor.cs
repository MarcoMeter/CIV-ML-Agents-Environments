using UnityEngine;

public class ArmMotor : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public float rotationSpeed;
    public float movementSpeed;

    public Vector3 MoveInput { get; set; }
    public float RotateInput { get; set; }


    private void FixedUpdate()
    {
        Move();
        LiftRotate();
    }

    private void Move()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.MovePosition(rigidbody.position + (MoveInput * movementSpeed * Time.fixedDeltaTime));
    }

    private void LiftRotate()
    {
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0f, RotateInput * rotationSpeed * Mathf.Deg2Rad, 0f));
    }
}
