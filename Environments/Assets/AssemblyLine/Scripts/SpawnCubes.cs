
namespace CIV_ML_Agents.AssemblyLine
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SpawnCubes : MonoBehaviour
    {
        #region Fields

        // Public Settings
        [SerializeField]
        private float _intervall = 1.0f;
        [SerializeField]
        private Transform[] _prefabs;
        [SerializeField]
        private float _moveSpeed = 3f;
        [SerializeField]
        private bool _resetCubes = true;

        // Private
        private float _nextSpawn = 0f;
        private float _npos = 0.0f;
        private List<GameObject> _cubes = new List<GameObject>();
        private int _numPrefabs = 0;

        #endregion


        #region Mono
        
        private void Start() => _numPrefabs = _prefabs.Length;
        
        private void Update()
        {
            if (Time.time >= _nextSpawn)
            {
                _nextSpawn = Time.time + _intervall;

                Transform cube = Instantiate(
                    original: _prefabs[Random.Range(0, _numPrefabs)].transform,
                    position: transform.position,
                    rotation: Quaternion.identity);

                _cubes.Add(cube.gameObject);
                _npos = Random.Range(-0.3f, 0.3f);
            }

            transform.localPosition = Vector3.Lerp(
                a: transform.localPosition,
                b: new Vector3(transform.localPosition.x, transform.localPosition.y, _npos),
                t: Time.deltaTime * _moveSpeed);
        }

        #endregion


        #region Functions

        public void ResetCubes()
        {
            if (_resetCubes)
            {
                for (int i = 0; i < _cubes.Count; i++)
                    Destroy(_cubes[i]);
                

                _cubes.Clear();
            }
        }

        public void Unregister(GameObject cube)
        {
            _cubes.Remove(cube);
            Destroy(cube);
        }

        #endregion
    }
}
