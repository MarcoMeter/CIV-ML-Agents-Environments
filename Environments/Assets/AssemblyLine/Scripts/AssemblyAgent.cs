
namespace CIV_ML_Agents.AssemblyLine
{
    using MLAgents;
    using UnityEngine;

    public class AssemblyAgent : Agent
    {
        #region Fields

        // Public Settings
        [SerializeField]
        private Rigidbody _mover;
        [SerializeField]
        private Vector3 _maxMoverDirection = new Vector3(0f, 0f, 1f);
        [SerializeField]
        private Transform _raySpawner;
        [SerializeField]
        private float _rayDistance = 0.1f;
        [SerializeField]
        private Vector2 _rayAmount = new Vector2(5, 5);
        [SerializeField]
        private SpawnCubes _spawner;

        // Private
        private Vector3 _moverOrigin;
        private RayPerception _rayPer;
        private float[] _movementBuckets = new float[] {
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


        #region Agent Lifecycle

        public override void InitializeAgent()
        {
            _rayPer = GetComponent<RayPerception>();
            _moverOrigin = _mover.transform.position;
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
                _rayPer.Perceive(
                    rayDistance: 12f,
                    rays: rays,
                    detectableObjects: detectableObjects,
                    startOffset: 0f,
                    endOffset: 0f));


            // Mover position
            AddVectorObs(_mover.transform.localPosition.z);
        }


        public override void AgentAction(float[] vectorAction, string textAction)
        {
            float actionY = _movementBuckets[(int)vectorAction[0]];

            _mover.MovePosition(
                Vector3.Lerp(
                    a: _mover.position,
                    b: _moverOrigin + (_maxMoverDirection * actionY),
                    t: Time.deltaTime * 10.0f));
        }


        public override void AgentReset()
        {
            _spawner.ResetCubes();
            _mover.position = _moverOrigin;
        }

        #endregion
    }
}
