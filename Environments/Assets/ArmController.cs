using UnityEngine;

public class ArmController : MonoBehaviour
{
    public ArmMotor motor;
    public Animator hookAnimator;
    public HookLogic hookLogic;

    private void Update()
    {
        // Movement
        var leftInput = default(Vector3);
        if (Input.GetKey(KeyCode.W))
            leftInput.z += 1;

        if (Input.GetKey(KeyCode.S))
            leftInput.z -= 1;

        if (Input.GetKey(KeyCode.A))
            leftInput.x -= 1;

        if (Input.GetKey(KeyCode.D))
            leftInput.x += 1;

        if (Input.GetKey(KeyCode.UpArrow))
            leftInput.y += 1;

        if (Input.GetKey(KeyCode.DownArrow))
            leftInput.y -= 1;
        motor.MoveInput = leftInput;


        // Rotation
        var rightInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            rightInput -= 1;

        if (Input.GetKey(KeyCode.RightArrow))
            rightInput += 1;
        motor.RotateInput = rightInput;


        // Open/Close hook
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool state = hookAnimator.GetBool("Closed");
            hookAnimator.SetBool("Closed", !state);
            hookLogic.Attach(state);
        }
    }
}
