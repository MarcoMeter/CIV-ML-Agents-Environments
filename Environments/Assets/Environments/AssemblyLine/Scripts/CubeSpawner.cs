namespace CIV_ML_Agents.AssemblyLine
{
    using System.Collections.Generic;
    using UnityEngine;


    public class CubeSpawner : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private float _spawnIntervall = 1.0f;
        [SerializeField]
        private Transform[] _cubePrefabs;
        [SerializeField]
        private float _moveSpeed = 3f;
        [SerializeField]
        [Tooltip("Maximal positive and negative spawner movement from origin")]
        private Vector3 _maxSpawnerMovement;
        [SerializeField]
        private bool _resetCubes = true;

        // Private Fields
        private float _nextSpawnTime = 0f;
        private Vector3 _spawnerOrigin;
        private Vector3 _newSpawnerPosition;
        private List<GameObject> _cubes = new List<GameObject>();
        private int _numPrefabs = 0;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _spawnerOrigin = transform.localPosition;
            _numPrefabs = _cubePrefabs.Length;
        }

        private void Update()
        {
            if (Time.time >= _nextSpawnTime)
            {
                _nextSpawnTime = Time.time + _spawnIntervall;

                Transform cube = Instantiate(
                    original: _cubePrefabs[Random.Range(0, _numPrefabs)].transform,
                    position: transform.position,
                    rotation: Quaternion.identity);

                _cubes.Add(cube.gameObject);
                _newSpawnerPosition = _spawnerOrigin + (_maxSpawnerMovement * Random.Range(-1f, 1f));
            }

            transform.localPosition = Vector3.Lerp(
                a: transform.localPosition,
                b: _newSpawnerPosition,
                t: Time.deltaTime * _moveSpeed);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Destroys all cubes and clears <see cref="_cubes"/>
        /// </summary>
        public void ResetCubes()
        {
            if (_resetCubes)
            {
                for (int i = 0; i < _cubes.Count; i++)
                    Destroy(_cubes[i]);

                _cubes.Clear();
            }
        }

        /// <summary>
        /// Destroys single cube
        /// </summary>
        /// <param name="cube">Cube to destroy</param>
        public void DestroyCube(GameObject cube)
        {
            _cubes.Remove(cube);
            Destroy(cube);
        }
        #endregion
    }

}