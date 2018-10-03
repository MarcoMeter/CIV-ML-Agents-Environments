using UnityEngine;

public class RobotCollisionCollector : MonoBehaviour
{
    public RobotAgent robot;

    public void CollisionCollect(Collider other)
    {
        print("RobotCollisionCollector");

        if (other.CompareTag("Enermy"))
            robot.Collision(false);
        else if (other.CompareTag("Goal"))
            robot.Collision(true);
    }
}
