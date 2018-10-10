using UnityEngine;

public class SendCollisionToParent : MonoBehaviour
{
    public RobotCollisionCollector robotCollisionCollector;

    private void OnTriggerEnter(Collider other)
    {
        //robotCollisionCollector.CollisionCollect(other);
    }
}