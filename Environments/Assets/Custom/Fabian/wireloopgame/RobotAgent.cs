using MLAgents;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class RobotAgent : Agent
{
	public CircularRayPerception circularRayPerception;
	public string[] detectabeObjects;
	public Transform ring;

	private RobotController robot;
	private RobotTelemetryData initialRobotPosition;

	private StayLevel ringStayLevel;
	internal Vector3 ringOldPos;
	internal Vector3 ringNewPos;
	internal Vector3 ringVelocity;
	internal float localXVelocity;

	private bool firstRun = true;
	public UnityEvent resetEvent;
	private Vector3 ringInitPos;
	private readonly float[] _movementBuckets = new float[] { -1.0f, -0.8f, -0.6f, -0.4f, -0.2f, 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };


	public override void InitializeAgent()
	{
		robot = GetComponent<RobotController>();
		ringStayLevel = GetComponentInChildren<StayLevel>();

		initialRobotPosition = new RobotTelemetryData(robot);
		ringInitPos = ring.position;
		ringOldPos = ringInitPos;
	}

	public override void CollectObservations()
	{
		AddVectorObs(circularRayPerception.Percieve(detectabeObjects));
		AddVectorObs(robot.GetTelemetry());
		// HACK Normalize velocity
		AddVectorObs(localXVelocity / 5f);
	}

	// Collision receiver
	internal void Collision(bool good)
	{
		if (good)
		{
			AddReward(1.0f);
		}
		else
		{
			AddReward(-1.0f);
		}

		Done();
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Prevents reward glitch on first frame
		if (firstRun)
		{
			ResetReward();
			Done();
			firstRun = false;
			return;
		}

		// Calculates Ring Velocity
		ringNewPos = ring.position;
		Vector3 media = ringNewPos - ringOldPos;
		ringVelocity = media / Time.fixedDeltaTime;
		ringOldPos = ringNewPos;

		// Adds reward depending on velocity
		// HACK Prevent velocity spikes on reset
		if (Mathf.Abs(localXVelocity) > 5f)
			Debug.LogWarning("Velocity exceeded clamp value");
		localXVelocity = Mathf.Clamp(ring.transform.InverseTransformVector(ringVelocity).x, -5f, 5f);

		AddReward(localXVelocity * 0.01f);

		// Time penality
		AddReward(-0.0001f);

		//Input Handling
		if (brain.brainType == BrainType.Player)
		{
			robot.inputBase = (Input.GetKey(KeyCode.KeypadPeriod) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad0) ? -1f : 0f);
			robot.inputBaseRotator = (Input.GetKey(KeyCode.Keypad3) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad2) ? -1f : 0f);
			robot.inputArm1 = (Input.GetKey(KeyCode.Keypad7) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad4) ? -1f : 0f);
			robot.inputArm2 = (Input.GetKey(KeyCode.Keypad8) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad5) ? -1f : 0f);
			robot.inputRing = (Input.GetKey(KeyCode.Keypad9) ? 1f : 0f) + (Input.GetKey(KeyCode.Keypad6) ? -1f : 0f);
		}
		else
		{
			robot.inputBase = _movementBuckets[(int)vectorAction[0]];
			robot.inputBaseRotator = _movementBuckets[(int)vectorAction[1]];
			robot.inputArm1 = _movementBuckets[(int)vectorAction[2]];
			robot.inputArm2 = _movementBuckets[(int)vectorAction[3]];
			robot.inputRing = _movementBuckets[(int)vectorAction[4]];
		}
	}

	public override void AgentOnDone()
	{
	}

	public override void AgentReset()
	{
		// Reset arm to init position
		robot.armBase = initialRobotPosition.armBase;
		robot.armBaseRotator = initialRobotPosition.armBaseRotator;
		robot.arm1 = initialRobotPosition.arm1;
		robot.arm2 = initialRobotPosition.arm2;
		robot.ring = initialRobotPosition.ring;

		// REVIEW Refresh Stay Level to prevent arm from glitching out
		ringStayLevel.FixedUpdate();

		resetEvent.Invoke();

		// REVIEW
		localXVelocity = 0f;
		ringNewPos = ringInitPos;
		ringOldPos = ringInitPos;
		ResetReward();
	}

	// Debug visuals
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(
			ring.position,
			ring.position + (ring.transform.right * localXVelocity));

		Handles.Label(ring.position, ring.position + "\n" + localXVelocity.ToString());
	}
#endif
}
