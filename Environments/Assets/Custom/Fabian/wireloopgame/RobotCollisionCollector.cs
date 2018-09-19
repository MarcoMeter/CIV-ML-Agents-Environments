using UnityEngine;

public class RobotCollisionCollector : MonoBehaviour
{
    public RobotAgent robot;

    public void CollisionCollect(Collider other)
    {
        if (other.CompareTag("Goal"))
            robot.Collision(true);

        else if (other.CompareTag("Enermy"))
            robot.Collision(false);
    }
}
