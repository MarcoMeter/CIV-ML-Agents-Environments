namespace CIV_ML_Agents.WireLoop
{
    using UnityEngine;
    using MLAgents;

    public class WireLoopAgent : Agent
    {
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
        private GenerateCurve _generateCurve;

        private string[] _detectableObjects = { "Enemy" };

        internal Vector3 _ringOldPos;
        internal Vector3 _ringNewPos;
        internal Vector3 _ringVelocity;
        internal float _localXVelocity;

        private JointDriveController _jdController;

        private bool _isNewDecisionStep;
        private int _currentDecisionStep;
        #endregion

        #region ML-Agents Lifecycle
        public override void InitializeAgent()
        {
            for (int i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].EvaluateStart();
                _rigs[i].Reset();
            }

            _jdController = GetComponent<JointDriveController>();
            _jdController.SetupBodyPart(_armBase.transform);
            _jdController.SetupBodyPart(_armLow.transform);
            _jdController.SetupBodyPart(_armHigh.transform);
            _jdController.SetupBodyPart(_armTop.transform);
            _jdController.SetupBodyPart(_armRotator.transform);
        }

        public override void AgentReset()
        {
            // Reset wire by generating a new curve
            _generateCurve.CalculateRandomPoints();
            _generateCurve.BuildMeshBezier();

            // Reset arm rig (position and velocities)
            for (int i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].Reset();
            }
            _ringNewPos = _ring.position;
            _ringOldPos = _ring.position;
            _ringVelocity = Vector3.zero;


            // Reset JointController target rotation and strength
            foreach (var bodyPart in _jdController.bodyPartsDict.Values)
            {
                bodyPart.Reset(bodyPart);
            }
        }

        public override void CollectObservations()
        {
            AddVectorObs(_circularRayPerception.Perceive(_detectableObjects));
            _jdController.GetCurrentJointForces();

            foreach (var bodyPart in _jdController.bodyPartsDict.Values)
            {
                CollectObservationBodyPart(bodyPart);
            }
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            // Calculates ring velocities
            _ringNewPos = _ring.position;
            Vector3 media = _ringNewPos - _ringOldPos;
            _ringVelocity = media / Time.fixedDeltaTime;
            _ringOldPos = _ringNewPos;


            // Reward for speed of ring on its local x-axis
            _localXVelocity = Mathf.Clamp(_ring.transform.InverseTransformVector(_ringVelocity).x, -5f, 5f);
            AddReward(_localXVelocity * 0.01f);

            // Punishment for each time step to motivate the agent to keep moving
            AddReward(-0.001f);

            if (_isNewDecisionStep)
            {

                var bpDict = _jdController.bodyPartsDict;

                // Set target rotation of body parts
                bpDict[_armBase.transform].SetJointTargetRotation(0f, 0f, vectorAction[0]);
                bpDict[_armLow.transform].SetJointTargetRotation(vectorAction[1], 0f, 0f);
                bpDict[_armHigh.transform].SetJointTargetRotation(vectorAction[2], 0f, 0f);
                bpDict[_armTop.transform].SetJointTargetRotation(vectorAction[3], 0f, 0f);
                bpDict[_armRotator.transform].SetJointTargetRotation(0f, vectorAction[4], 0f);

                // Set joint strength of body parts (speed at which it can move)
                bpDict[_armBase.transform].SetJointStrength(vectorAction[5]);
                bpDict[_armLow.transform].SetJointStrength(vectorAction[6]);
                bpDict[_armHigh.transform].SetJointStrength(vectorAction[7]);
                bpDict[_armTop.transform].SetJointStrength(vectorAction[8]);
                bpDict[_armRotator.transform].SetJointStrength(vectorAction[9]);
            }

            IncrementDecisionTimer();
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Collects the observations for a single body part. Including (ground contact, local velocity, angular velocity, local rotation, local position to base)
        /// </summary>
        /// <param name="bp"></param>
        private void CollectObservationBodyPart(BodyPart bp)
        {
            var rb = bp.rb;
            AddVectorObs(bp.groundContact.touchingGround ? 1 : 0); // Is this body part touching the ground?
            AddVectorObs(rb.velocity);
            AddVectorObs(rb.angularVelocity);
            Vector3 localPosRelToBase = _armBase.transform.InverseTransformPoint(rb.position);
            AddVectorObs(localPosRelToBase);

            if (bp.rb.transform != _armBase)
            {
                AddVectorObs(bp.currentXNormalizedRot);
                AddVectorObs(bp.currentYNormalizedRot);
                AddVectorObs(bp.currentZNormalizedRot);
                AddVectorObs(bp.currentStrength / _jdController.maxJointForceLimit);
            }
        }

        /// <summary>
        /// Increments decision step, so that not every update changes the target rotation of the body parts.
        /// </summary>
        private void IncrementDecisionTimer()
        {
            if (_currentDecisionStep == agentParameters.numberOfActionsBetweenDecisions ||
                agentParameters.numberOfActionsBetweenDecisions == 1)
            {
                _currentDecisionStep = 1;
                _isNewDecisionStep = true;
            }
            else
            {
                _currentDecisionStep++;
                _isNewDecisionStep = false;
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Punishes the agent and stops the episode.
        /// </summary>
        public void Loose()
        {
            AddReward(-1f);
            Done();
        }

        /// <summary>
        /// Rewards the agent and stops the episode.
        /// </summary>
        public void Win()
        {
            AddReward(1f);
            Done();
        }
        #endregion
    }
}