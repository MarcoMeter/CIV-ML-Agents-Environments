namespace CIV_ML_Agents.AssemblyLine
{
    using UnityEngine;

    public class ConveyorBelt : MonoBehaviour
    {
        #region Member Fields
        // Inspector
        [SerializeField]
        private Vector3 _speed;
        
        private Rigidbody _rig;
        private ConstantForce _cf;

        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _rig = GetComponent<Rigidbody>();
            _cf = GetComponent<ConstantForce>();
        }
        
        private void OnCollisionStay(Collision collision) => collision.rigidbody.MovePosition(collision.transform.position + (_speed * Time.fixedDeltaTime));

        private void OnCollisionExit(Collision collision) => collision.rigidbody.AddForce(_speed * collision.rigidbody.mass, ForceMode.Impulse);
        #endregion
    }
}