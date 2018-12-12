namespace CIV_ML_Agents.WireLoop
{
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class LooseCollision : MonoBehaviour
    {
        #region Member Fields
        public WireLoopAgent wireloopAgent;
        #endregion

        #region Unity Lifecycle
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                wireloopAgent.Loose();
            }
        }
        #endregion
    }
}