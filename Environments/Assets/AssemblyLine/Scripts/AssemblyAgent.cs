namespace CIV_ML_Agents.AssemblyLine
{
    using MLAgents;
    using UnityEngine;

    public class AssemblyAgent : Agent
    {
        #region Member Fields
        // Inspector
        [Header("References")]
        [SerializeField]
        [Tooltip("Raycast origin point")]
        private Rigidbody _pusher;
        [SerializeField]
        private Transform _raySpawner;
        [SerializeField]
        private RayPerception _rayPerception;
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
        private float _rayDistance;
        [SerializeField]
        private float _raySpacing;
        [SerializeField]
        private Vector2 _rayAmount = new Vector2(5, 5);

        // Privates
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
        #endregion

        #region ML-Agents Lifecycle
        public override void InitializeAgent() => _pusherOrigin = _pusher.transform.position;

        public override void AgentReset()
        {
            _cubeSpawner.ResetCubes();
            _pusher.position = _pusherOrigin;
        }

        public override void CollectObservations()
        {
            // Generate array of raycasts
            var rays = new Ray[(int)_rayAmount.x * (int)_rayAmount.y];

            float halfX = ((int)_rayAmount.x - 1) * _rayDistance / 2.0f;
            float halfY = ((int)_rayAmount.y - 1) * _rayDistance / 2.0f;

            string[] detectableObjects = { "blueBox", "redBox", "Untagged" };

            var rot = Matrix4x4.Rotate(_raySpawner.rotation);


            // Setup raycast array
            for (int x = 0; x < (int)_rayAmount.x; x++)
            {
                for (int y = 0; y < (int)_rayAmount.y; y++)
                {
                    var origin = new Vector3(
                        x: (x * _rayDistance) - halfX,
                        y: (y * _rayDistance) - halfY,
                        z: 0f);

                    var direction = new Vector3(0, 0, 1f);

                    origin = rot.MultiplyPoint3x4(origin);
                    direction = rot.MultiplyPoint3x4(direction);
                    origin += _raySpawner.position;

                    var ray = new Ray(origin, direction);

                    rays[(x * (int)_rayAmount.x) + y] = ray;
                }
            }

            // Raycast return
            AddVectorObs(
                _rayPerception.Perceive(
                    rayDistance: 12f,
                    rays: rays,
                    detectableObjects: detectableObjects,
                    startOffset: 0f,
                    endOffset: 0f));

            // Mover position
            AddVectorObs(_pusher.transform.localPosition.z);
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            float moveAction = _movementBuckets[(int)vectorAction[0]];

            _pusher.MovePosition(
                Vector3.Lerp(
                    a: _pusher.position,
                    b: _pusherOrigin + (_maxPusherMovement * moveAction),
                    t: Time.deltaTime * _pusherSpeed));
        }
        #endregion
    }
}