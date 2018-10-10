using UnityEngine;

public class RobotCollisionCollector : MonoBehaviour
{
    public RobotAgent robot;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            robot.Collision(true);
        } else if (other.CompareTag("Enemy"))
        {
            robot.Collision(false);
        }
    }
}
