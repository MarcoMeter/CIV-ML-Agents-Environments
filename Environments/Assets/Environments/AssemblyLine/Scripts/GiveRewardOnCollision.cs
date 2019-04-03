namespace CIV_ML_Agents.AssemblyLine
{
    using MLAgents;
    using UnityEngine;

    public class GiveRewardOnCollision : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private string _acceptedTag;
        [SerializeField]
        private float _reward = 1f;
        [SerializeField]
        private float _punish = -1f;
        [SerializeField]
        private Agent _agent;
        [SerializeField]
        private CubeSpawner _spawner;
        #endregion

        #region Unity Lifecycle        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.gameObject.tag == _acceptedTag)
                _agent.AddReward(_reward);
            else
                _agent.AddReward(_punish);

            _spawner.DestroyCube(collision.transform.gameObject);
        }
        #endregion
    }
}