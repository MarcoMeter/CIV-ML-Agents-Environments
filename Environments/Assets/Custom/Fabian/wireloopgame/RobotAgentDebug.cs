using System;
using UnityEngine;
using UnityEngine.Events;

public class RobotAgentDebug : MonoBehaviour
{
    public float currentReward, highestReward;
    public float currentTotalReward, highestTotalReward;
	public float currentRingVelocity, highestRingVelocity;
    private RobotAgent robot;

    private void Start()
    {
        robot = GetComponent<RobotAgent>();
		robot.resetEvent.AddListener(OnAgentReset);
    }

	private void OnAgentReset() => Debug.Log(Time.frameCount + " Reset");

	private void FixedUpdate()
    {
        currentReward = robot.GetReward();
        currentTotalReward = robot.GetCumulativeReward();
		currentRingVelocity = robot.localXVelocity;

        if (currentReward > highestReward)
        {
            highestReward = currentReward;
		}

		if (currentTotalReward > highestTotalReward)
		{
			highestTotalReward = currentTotalReward;
		}

		if (currentRingVelocity > highestRingVelocity)
		{
			highestRingVelocity = currentRingVelocity;
		}
		
		if(currentRingVelocity > 20f)
		{
			// DEBUG
			Debug.Log(Time.frameCount + " Break");

			Debug.Log(robot.ringOldPos);
			Debug.Log(robot.ring.position);
			Debug.Log(robot.ringNewPos);

			Debug.Break();
		}
	}

    private void OnGUI()
    {
		// DEBUG
        GUI.color = Color.black;
        GUILayout.Label("Current Reward: " + currentReward);
        GUILayout.Label("Highest Reward: " + highestReward);
        GUILayout.Label("Current Velocity: " + currentTotalReward);
        GUILayout.Label("Highest Total Reward: " + highestTotalReward);
		GUILayout.Label("Current Ring Velocity: " + currentRingVelocity);
		GUILayout.Label("Highest Ring Velocity: " + highestRingVelocity);
	}
}