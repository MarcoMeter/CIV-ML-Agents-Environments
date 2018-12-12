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
        private float _force = 2f;
        private readonly float[] _movementBuckets = new float[] { -1.0f, -0.1f, 0f, 0.1f, 1.0f };

        private static string[] detectableObjects = { "Enemy" };

        internal Vector3 _ringOldPos;
        internal Vector3 _ringNewPos;
        internal Vector3 _ringVelocity;
        internal float _localXVelocity;

        JointDriveController jdController;

        bool isNewDecisionStep;
        int currentDecisionStep;


        #endregion

        #region ML-Agents Lifecycle
        public override void InitializeAgent()
        {
            for (int i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].EvaluateStart();
                _rigs[i].Reset();
            }

            //_armBase.gameObject.GetComponent<Renderer>().material

            jdController = GetComponent<JointDriveController>();
            jdController.SetupBodyPart(_armBase.transform);
            jdController.SetupBodyPart(_armLow.transform);
            jdController.SetupBodyPart(_armHigh.transform);
            jdController.SetupBodyPart(_armTop.transform);
            jdController.SetupBodyPart(_armRotator.transform);
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

            foreach (var bodyPart in jdController.bodyPartsDict.Values)
            {
                bodyPart.Reset(bodyPart);
            }
        }

        public void CollectObservationBodyPart(BodyPart bp)
        {
            var rb = bp.rb;
            AddVectorObs(bp.groundContact.touchingGround ? 1 : 0); // Is this bp touching the ground
            AddVectorObs(rb.velocity);
            AddVectorObs(rb.angularVelocity);
            Vector3 localPosRelToBase = _armBase.transform.InverseTransformPoint(rb.position);
            AddVectorObs(localPosRelToBase);

            if (bp.rb.transform != _armBase)
            {
                AddVectorObs(bp.currentXNormalizedRot);
                AddVectorObs(bp.currentYNormalizedRot);
                AddVectorObs(bp.currentZNormalizedRot);
                AddVectorObs(bp.currentStrength / jdController.maxJointForceLimit);
            }
        }

        public override void CollectObservations()
        {
            /*AddVectorObs(_circularRayPerception.Perceive(detectableObjects));
            AddVectorObs(_armBase.transform.localRotation);
            AddVectorObs(_armLow.transform.localRotation);
            AddVectorObs(_armHigh.transform.localRotation);
            AddVectorObs(_armTop.transform.localRotation);
            AddVectorObs(_armRotator.transform.localRotation);

            // Velocities
            AddVectorObs(_armBase.angularVelocity);
            AddVectorObs(_armLow.angularVelocity);
            AddVectorObs(_armHigh.angularVelocity);
            AddVectorObs(_armTop.angularVelocity);
            AddVectorObs(_armRotator.angularVelocity);*/

            AddVectorObs(_circularRayPerception.Perceive(detectableObjects));
            jdController.GetCurrentJointForces();

            foreach (var bodyPart in jdController.bodyPartsDict.Values)
            {
                CollectObservationBodyPart(bodyPart);
            }
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            // Calculates Ring Velocities
            _ringNewPos = _ring.position;
            Vector3 media = _ringNewPos - _ringOldPos;
            _ringVelocity = media / Time.fixedDeltaTime;
            _ringOldPos = _ringNewPos;

            _localXVelocity = Mathf.Clamp(_ring.transform.InverseTransformVector(_ringVelocity).x, -5f, 5f);
            AddReward(_localXVelocity * 0.01f);
            AddReward(-0.001f);

            /*_armBase.AddRelativeTorque(new Vector3(0f, 0f, 10f) * _movementBuckets[(int)vectorAction[0]] * _force, ForceMode.Impulse);
            _armLow.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[1]] * _force, ForceMode.Impulse);
            _armHigh.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[2]] * _force, ForceMode.Impulse);
            _armTop.AddRelativeTorque(new Vector3(10f, 0f, 0f) * _movementBuckets[(int)vectorAction[3]] * _force, ForceMode.Impulse);
            _armRotator.AddRelativeTorque(new Vector3(0f, 10f, 0f) * _movementBuckets[(int)vectorAction[4]] * _force, ForceMode.Impulse);*/

            if (isNewDecisionStep)
            {

                var bpDict = jdController.bodyPartsDict;

                bpDict[_armBase.transform].SetJointTargetRotation(0f, 0f, vectorAction[0]);
                bpDict[_armLow.transform].SetJointTargetRotation(vectorAction[1], 0f, 0f);
                bpDict[_armHigh.transform].SetJointTargetRotation(vectorAction[2], 0f, 0f);
                bpDict[_armTop.transform].SetJointTargetRotation(vectorAction[3], 0f, 0f);
                bpDict[_armRotator.transform].SetJointTargetRotation(0f, vectorAction[4], 0f);

                bpDict[_armBase.transform].SetJointStrength(vectorAction[5]);
                bpDict[_armLow.transform].SetJointStrength(vectorAction[6]);
                bpDict[_armHigh.transform].SetJointStrength(vectorAction[7]);
                bpDict[_armTop.transform].SetJointStrength(vectorAction[8]);
                bpDict[_armRotator.transform].SetJointStrength(vectorAction[9]);

            }

            IncrementDecisionTimer();
        }

        public void IncrementDecisionTimer()
        {
            if (currentDecisionStep == agentParameters.numberOfActionsBetweenDecisions ||
                agentParameters.numberOfActionsBetweenDecisions == 1)
            {
                currentDecisionStep = 1;
                isNewDecisionStep = true;
            }
            else
            {
                currentDecisionStep++;
                isNewDecisionStep = false;
            }
        }


        #endregion

        #region Public Functions
        /// <summary>
        /// 
        /// </summary>
        public void Loose()
        {
            AddReward(-1f);
            Done();
        }
        #endregion
    }
}