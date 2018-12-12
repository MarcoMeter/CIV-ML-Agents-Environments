namespace CIV_ML_Agents.AssemblyLine
{
    using UnityEngine;

    public class AssemblyLineMover : MonoBehaviour
    {
        #region Member Fields
        // Inspector
        [SerializeField]
        private float _speed = 30f;
        
        private Rigidbody _rig;
        private ConstantForce _cf;

        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _rig = GetComponent<Rigidbody>();
            _cf = GetComponent<ConstantForce>();
        }
        
        private void OnCollisionStay(Collision collision) => collision.rigidbody.MovePosition(collision.transform.position + (Vector3.right * _speed * Time.deltaTime));

        private void OnCollisionExit(Collision collision) => collision.rigidbody.AddForce(Vector3.right * _speed * collision.rigidbody.mass, ForceMode.Impulse);
        #endregion
    }
}