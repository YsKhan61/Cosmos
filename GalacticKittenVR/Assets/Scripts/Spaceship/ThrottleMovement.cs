using UnityEngine;
using UnityEngine.Serialization;

namespace GalacticKittenVR.Spaceship
{
    /// <summary>
    /// The movement caused by the throttle handle
    /// </summary>
    public class ThrottleMovement : MonoBehaviour
    {
        public float ThrottleValue { get; set; }

        [SerializeField, Tooltip("The rigidbody to move")]
        private Rigidbody _rigidbody = null;

        [SerializeField, Tooltip("The max force to input to the rigidbody")]
        private float _maxForceToInput = 1000f;

        [SerializeField, Tooltip("The max force to allow to the rigidbody")]
        [FormerlySerializedAs("_maxForceAllowed")]
        private float _maxLinearVelocityAllowed = 1000f;

        [SerializeField, Tooltip("Damping factor at 0 force")]
        private float _damping = 5f;

        private void Start()
        {
            _rigidbody.maxLinearVelocity = _maxLinearVelocityAllowed;
        }

        private void FixedUpdate()
        {
            // Move the rigidbody forward or backward with a force proportional to the throttle value
            _rigidbody.AddForce(transform.forward * ThrottleValue * _maxForceToInput * Time.fixedDeltaTime);

            if (ThrottleValue == 0)
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, _damping * Time.fixedDeltaTime);
        }
    }
}

