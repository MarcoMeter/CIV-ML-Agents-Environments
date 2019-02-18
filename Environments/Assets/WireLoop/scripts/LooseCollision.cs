namespace CIV_ML_Agents.WireLoop
{
    using UnityEngine;

    /// <summary>
    /// Determines wether the agent looses or wins based on collisions with the wire, the starting base and the goal.
    /// </summary>
    public class LooseCollision : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private WireLoopAgent _wireloopAgent;
        #endregion

        #region Unity Lifecycle
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                _wireloopAgent.Loose();
            }
            if(collision.gameObject.tag == "Goal")
            {
                _wireloopAgent.Win();
            }
        }
        #endregion
    }
}