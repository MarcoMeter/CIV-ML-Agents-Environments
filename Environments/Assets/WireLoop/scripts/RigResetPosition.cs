namespace CIV_ML_Agents.WireLoop
{
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class RigResetPosition : MonoBehaviour
    {
        #region Member Fields
        public Vector3 position;
        public Vector3 rotation;
        public Rigidbody rig;

        public bool startPosition = false;
        public bool startRotation = false;
        #endregion

        #region Public Functions
        /// <summary>
        /// 
        /// </summary>
        public void EvaluateStart()
        {
            if (startPosition)
            {
                position = rig.transform.localPosition;
            }

            if (startRotation)
            {
                rotation = rig.transform.localEulerAngles;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            rig.isKinematic = true;
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;
            rig.transform.localPosition = position;
            rig.transform.localEulerAngles = rotation;
            rig.isKinematic = false;
        }
        #endregion
    }
}