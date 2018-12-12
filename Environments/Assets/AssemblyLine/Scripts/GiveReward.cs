namespace CIV_ML_Agents.AssemblyLine
{
    using MLAgents;
    using UnityEngine;

    public class GiveReward : MonoBehaviour
    {
        #region Member Fields
        // Inspector
        [SerializeField]
        private float _reward = 1f;
        [SerializeField]
        private float _punish = 0f;
        [SerializeField]
        private string _acceptedClass = "blueClass";
        [SerializeField]
        private Agent _agent;
        [SerializeField]
        private SpawnCubes _spawner;
        #endregion

        #region Unity Lifecycle        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.gameObject.tag == _acceptedClass)
                _agent.AddReward(_reward);

            else
                _agent.AddReward(-_punish);

            _spawner.Unregister(collision.transform.gameObject);
        }
        #endregion
    }
}