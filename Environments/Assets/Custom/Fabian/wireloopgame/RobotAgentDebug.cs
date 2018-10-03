using UnityEngine;

public class RobotAgentDebug : MonoBehaviour
{
    public float currentReward, highestReward;
    public float currentTotalReward, highestTotalReward;
    private RobotAgent robot;

    private void Start()
    {
        robot = GetComponent<RobotAgent>();
    }

    private void FixedUpdate()
    {
        currentReward = robot.GetReward();
        currentTotalReward = robot.GetCumulativeReward();

        if (currentReward > highestReward)
        {
            highestReward = currentReward;
        }

        if (currentTotalReward > highestTotalReward)
        {
            highestTotalReward = currentTotalReward;
        }
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUILayout.Label("currentReward " + currentReward);
        GUILayout.Label("highestReward: " + highestReward);
        GUILayout.Label("currentTotalReward: " + currentTotalReward);
        GUILayout.Label("highestTotalReward: " + highestTotalReward);
    }
}