using UnityEngine;


namespace Cosmos.Spaceship
{
    /// <summary>
    /// The movement caused by the control handle
    /// </summary>
    public class ControlMovement : MonoBehaviour
    {
        [SerializeField, Tooltip("The orientation along which to apply torque")]
        private Vector3 _torqueDirection;
        public Vector3 TorqueDirection { set => _torqueDirection = value; }

        [SerializeField, Tooltip("The factor by which the torque is multiplied")]
        private float _torqueMultiplier = 1f;
        public float TorqueMultiplier { set => _torqueMultiplier = value; }

        [SerializeField, Tooltip("The max torque to input to the rigidbody")]
        private float _maxTorqueToInput = 1000f;

        [SerializeField, Tooltip("The max torque to allow to the rigidbody")]
        private float _maxTorqueAllowed = 1000f;

        [SerializeField, Tooltip("The rigidbody to move")]
        private Rigidbody _rigidbody = null;

        [SerializeField, Tooltip("Damping factor at 0 torque")]
        private float _damping = 100f;

        private void Start()
        {
            _rigidbody.maxAngularVelocity = _maxTorqueAllowed;
        }

        private void FixedUpdate()
        {
            // Rotate the rigidbody along the orientation with a torque proportional to the throttle value
            _rigidbody.AddTorque(transform.TransformDirection(_torqueDirection) * _torqueMultiplier * _maxTorqueToInput * Time.fixedDeltaTime);

            if(_torqueDirection == Vector3.zero)
                _rigidbody.angularVelocity = Vector3.Slerp(_rigidbody.angularVelocity, Vector3.zero, _damping * Time.fixedDeltaTime);
        }
    }

}
