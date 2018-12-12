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
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                //Debug.Log("Collision");
                wireloopAgent.Loose();
            }
        }
        #endregion
    }
}