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

        [SerializeField]
        private GenerateCurve _generateCurve;

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

            jdController = GetComponent<JointDriveController>();
            jdController.SetupBodyPart(_armBase.transform);
            jdController.SetupBodyPart(_armLow.transform);
            jdController.SetupBodyPart(_armHigh.transform);
            jdController.SetupBodyPart(_armTop.transform);
            jdController.SetupBodyPart(_armRotator.transform);
        }

        public override void AgentReset()
        {
            // Neuen Draht erstellen
            _generateCurve.CalculateRandomPoints();
            _generateCurve.BuildMeshBezier();


            // Körperteile auf ausgangsposition zurücksetzen.
            for (int i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].Reset();
            }
            _ringNewPos = _ring.position;
            _ringOldPos = _ring.position;
            _ringVelocity = Vector3.zero;


            // Joint Controller zurücksetzen
            foreach (var bodyPart in jdController.bodyPartsDict.Values)
            {
                bodyPart.Reset(bodyPart);
            }
        }


        // Observation für ein einzelnes Körperteil
        public void CollectObservationBodyPart(BodyPart bp)
        {
            Rigidbody rb = bp.rb;

            // Bodenkontakt des Körperteils
            AddVectorObs(bp.groundContact.touchingGround ? 1 : 0); // Is this bp touching the ground

            // Geschwindigkeit des Körperteils
            AddVectorObs(rb.velocity);

            // Winkelgeschwindigkeit des Körperteils
            AddVectorObs(rb.angularVelocity);

            // Lokale Position des Körperteils im Verhältnis zur Basis des Roboterarms
            Vector3 localPosRelToBase = _armBase.transform.InverseTransformPoint(rb.position);
            AddVectorObs(localPosRelToBase);

            // Wenn Körperteil nicht Basis ist, dann lokale Rotationen des Körperteils
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
            // Raycasts vom Ring aus nach innen über CircularRayPerceptor
            AddVectorObs(_circularRayPerception.Perceive(detectableObjects));

            // JointController werte ermitteln
            jdController.GetCurrentJointForces();


            // Für jedes Körperteil observationen durchführen
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


            // Reward für Geschwindigkeit des Rings in der lokalen X-Achse abhängig von der Rotation
            _localXVelocity = Mathf.Clamp(_ring.transform.InverseTransformVector(_ringVelocity).x, -5f, 5f);
            AddReward(_localXVelocity * 0.01f);

            // Abzüge fürs Überleben
            AddReward(-0.001f);

            if (isNewDecisionStep)
            {

                var bpDict = jdController.bodyPartsDict;

                // Zielrotationen für Körperteile festlegen.
                bpDict[_armBase.transform].SetJointTargetRotation(0f, 0f, vectorAction[0]);
                bpDict[_armLow.transform].SetJointTargetRotation(vectorAction[1], 0f, 0f);
                bpDict[_armHigh.transform].SetJointTargetRotation(vectorAction[2], 0f, 0f);
                bpDict[_armTop.transform].SetJointTargetRotation(vectorAction[3], 0f, 0f);
                bpDict[_armRotator.transform].SetJointTargetRotation(0f, vectorAction[4], 0f);

                // Kraft der einzelnen Körperteile festlegen
                bpDict[_armBase.transform].SetJointStrength(vectorAction[5]);
                bpDict[_armLow.transform].SetJointStrength(vectorAction[6]);
                bpDict[_armHigh.transform].SetJointStrength(vectorAction[7]);
                bpDict[_armTop.transform].SetJointStrength(vectorAction[8]);
                bpDict[_armRotator.transform].SetJointStrength(vectorAction[9]);

            }

            IncrementDecisionTimer();
        }

        /**
         * IncrementDecisionTimer() sorgt dafür, dass zwar jeden step ein Reward ausgesprochen werden kann,
         * aber nicht jeden step die Zielrotationen geändert werden können.
         * **/
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

        public void Win()
        {
            AddReward(1f);
            Done();
        }
        #endregion
    }
}
