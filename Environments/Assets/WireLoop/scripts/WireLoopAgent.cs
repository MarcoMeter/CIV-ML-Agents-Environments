using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WireLoopAgent : Agent {

    #region Member Fields

    [SerializeField]
    private CircularRayPerception _circularRayPerception;
    [SerializeField]
    private RigResetPosition[] _rigs;

    [SerializeField]
    private Rigidbody _armBase;
    [SerializeField]
    private Rigidbody _armLow;
    [SerializeField]
    private Rigidbody _armHigh;
    [SerializeField]
    private Rigidbody _armTop;
    [SerializeField]
    private Rigidbody _armRotator;

    [SerializeField]
    private Transform _ring;

    [SerializeField]
    private float _force = 2f;
    private readonly float[] _movementBuckets = new float[] { -1.0f, -0.1f, 0f, 0.1f, 1.0f };

    private static string[] detectableObjects = {"Enemy"};

    internal Vector3 _ringOldPos;
    internal Vector3 _ringNewPos;
    internal Vector3 _ringVelocity;
    internal float _localXVelocity;

    #endregion

    #region Member Lifecycle

    public override void InitializeAgent()
    {
        for(int i = 0; i < _rigs.Length; i++)
        {
            _rigs[i].EvaluateStart();
            _rigs[i].Reset();
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(_circularRayPerception.Perceive(detectableObjects));
        AddVectorObs(_armBase.transform.localRotation);
        AddVectorObs(_armLow.transform.localRotation);
        AddVectorObs(_armHigh.transform.localRotation);
        AddVectorObs(_armTop.transform.localRotation);
        AddVectorObs(_armRotator.transform.localRotation);

        //velocitys
        AddVectorObs(_armBase.angularVelocity);
        AddVectorObs(_armLow.angularVelocity);
        AddVectorObs(_armHigh.angularVelocity);
        AddVectorObs(_armTop.angularVelocity);
        AddVectorObs(_armRotator.angularVelocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Calculates Ring Velocity
        _ringNewPos = _ring.position;
        Vector3 media = _ringNewPos - _ringOldPos;
        _ringVelocity = media / Time.fixedDeltaTime;
        _ringOldPos = _ringNewPos;

        _localXVelocity = Mathf.Clamp(_ring.transform.InverseTransformVector(_ringVelocity).x, -5f, 5f);
        AddReward(_localXVelocity * 0.01f);
        AddReward(-0.001f);

        _armBase.AddRelativeTorque(new Vector3(0f, 0f, 10f) * _movementBuckets[(int)vectorAction[1]] * _force, ForceMode.Impulse);
        _armLow.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[2]] * _force, ForceMode.Impulse);
        _armHigh.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[3]] * _force, ForceMode.Impulse);
        _armTop.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[4]] * _force, ForceMode.Impulse);
        _armRotator.AddRelativeTorque(new Vector3(0f, 10f, 0f) * _movementBuckets[(int)vectorAction[5]] * _force, ForceMode.Impulse);
    }

    public override void AgentReset()
    {
        for (int i = 0; i < _rigs.Length; i++)
        {
            _rigs[i].Reset();
        }
        _ringNewPos = _ring.position;
        _ringOldPos = _ring.position;
        _ringVelocity = Vector3.zero;
    }

    public void Loose()
    {
        AddReward(-1f);
        Done();
    }

    #endregion

}