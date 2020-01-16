namespace CIV_ML_Agents.AssemblyLine
{
    using MLAgents;
    using UnityEngine;

    public class AssemblyAgent : Agent
    {
        #region Member Fields
        [Header("References")]
        [SerializeField]
        private Rigidbody _pusher;
        [SerializeField]
        private Transform _raySpawnOrigin;
        [SerializeField]
        private RayPerceptionOld _rayPerception;
        [SerializeField]
        private CubeSpawner _cubeSpawner;

        [Header("Pusher")]
        [SerializeField]
        [Tooltip("Maximal positive and negative pusher movement from origin")]
        private Vector3 _maxPusherMovement;
        [SerializeField]
        private float _pusherSpeed = 1f;

        [Header("Raycasting")]
        [SerializeField]
        private Vector2 _rayAmount = new Vector2(5, 5);
        [SerializeField]
        private float _raySpacing;
        [SerializeField]
        private float _rayLength;
        [SerializeField]
        private float _rayThickness;
        [SerializeField]
        private string[] _detectableObjects;
        [SerializeField]
        private LayerMask _physicsLayer;

        // Private Fields
        private Vector3 _pusherOrigin;
        private float[] _movementBuckets = new float[]
        {
            -1.0f,
            -0.8f,
            -0.6f,
            -0.4f,
            -0.2f,
            0.0f,
            0.2f,
            0.4f,
            0.6f,
            0.8f,
            1.0f
        };
        private Ray[] _rayArray;
        #endregion

        #region ML-Agents Lifecycle
        public override void InitializeAgent()
        {
            _pusherOrigin = _pusher.transform.position;

            // Generate array of raycasts
            _rayArray = new Ray[(int)_rayAmount.x * (int)_rayAmount.y];

            float halfX = ((int)_rayAmount.x - 1) * _raySpacing / 2.0f;
            float halfY = ((int)_rayAmount.y - 1) * _raySpacing / 2.0f;

            var rot = Matrix4x4.Rotate(_raySpawnOrigin.rotation);

            // Setup raycast array
            for (int x = 0; x < (int)_rayAmount.x; x++)
            {
                for (int y = 0; y < (int)_rayAmount.y; y++)
                {
                    var origin = new Vector3(
                        x: (x * _raySpacing) - halfX,
                        y: (y * _raySpacing) - halfY,
                        z: 0f);

                    var direction = new Vector3(0, 0, 1);

                    origin = rot.MultiplyPoint3x4(origin);
                    direction = rot.MultiplyPoint3x4(direction);
                    origin += _raySpawnOrigin.position;

                    var ray = new Ray(origin, direction);

                    _rayArray[(x * (int)_rayAmount.x) + y] = ray;
                }
            }
        }

        public override void AgentReset()
        {
            _cubeSpawner.ResetCubes();
            _pusher.position = _pusherOrigin;
        }

        public override void CollectObservations(){ }

        public override void AgentAction(float[] vectorAction)
        {
            float moveAction = _movementBuckets[(int)vectorAction[0]];

            _pusher.MovePosition(
                Vector3.Lerp(
                    a: _pusher.position,
                    b: _pusherOrigin + (_maxPusherMovement * moveAction),
                    t: Time.deltaTime * _pusherSpeed));
        }

        public override float[] Heuristic()
        {
            var action = new float[1];

            action[0] = 5f;
            return action;
        }
        #endregion
    }
}