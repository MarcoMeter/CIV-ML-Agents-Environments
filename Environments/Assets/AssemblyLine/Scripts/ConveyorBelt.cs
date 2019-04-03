namespace CIV_ML_Agents.AssemblyLine
{
    using UnityEngine;

    public class ConveyorBelt : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private Vector3 _speed;

        #endregion

        #region Unity Lifecycle
        private void OnCollisionStay(Collision collision) => collision.rigidbody.MovePosition(collision.transform.position + (_speed * Time.fixedDeltaTime));

        private void OnCollisionExit(Collision collision) => collision.rigidbody.AddForce(_speed * collision.rigidbody.mass, ForceMode.Impulse);
        #endregion
    }
}