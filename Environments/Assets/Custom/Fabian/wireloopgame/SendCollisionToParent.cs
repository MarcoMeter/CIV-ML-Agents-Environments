using UnityEngine;

public class SendCollisionToParent : MonoBehaviour
{
    public RobotCollisionCollector robotCollisionCollector;

    private void OnTriggerEnter(Collider other)
    {
        print("SendCollisionToParent");
        robotCollisionCollector.CollisionCollect(other);
    }
}