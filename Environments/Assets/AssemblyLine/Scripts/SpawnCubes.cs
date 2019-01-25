
namespace CIV_ML_Agents.AssemblyLine
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    [Obsolete("Please use CubeSpawner instead", true)]
    public class SpawnCubes : MonoBehaviour
    {
        #region Member Fields
        // Inspector
        [SerializeField]
        private float _intervall = 1.0f;
        [SerializeField]
        private Transform[] _prefabs;
        [SerializeField]
        private float _moveSpeed = 3f;
        [SerializeField]
        private bool _resetCubes = true;
        [SerializeField]
        private float posRange = 0.3f;

        private float _nextSpawn = 0f;
        private float _npos = 0.0f;
        private List<GameObject> _cubes = new List<GameObject>();
        private int _numPrefabs = 0;
        #endregion

        #region Unity Lifecycle
        private void Start() => _numPrefabs = _prefabs.Length;

        private void Update()
        {
            if (Time.time >= _nextSpawn)
            {
                _nextSpawn = Time.time + _intervall;

                Transform cube = Instantiate(
                    original: _prefabs[UnityEngine.Random.Range(0, _numPrefabs)].transform,
                    position: transform.position,
                    rotation: Quaternion.identity);

                _cubes.Add(cube.gameObject);
                _npos = UnityEngine.Random.Range(-posRange, posRange);
            }

            transform.localPosition = Vector3.Lerp(
                a: transform.localPosition,
                b: new Vector3(transform.localPosition.x, _npos, transform.localPosition.z),
                t: Time.deltaTime * _moveSpeed);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="cube"></param>
        public void Unregister(GameObject cube)
        {
            _cubes.Remove(cube);
            Destroy(cube);
        }
        #endregion
    }
}