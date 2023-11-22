using UnityEngine;


namespace GalacticKittenVR.Spaceship
{
    /// <summary>
    /// The movement caused by the throttle
    /// </summary>
    public class ThrottleMovement : MonoBehaviour
    {
        [SerializeField, Range(-1f, 1f), Tooltip("The value of current throttle")]
        private float _throttleValue = 0.0f;
        public float SetThrottleValue { set => _throttleValue = value; }

        [SerializeField, Tooltip("The rigidbody to move")]
        private Rigidbody _rigidbody = null;

        [SerializeField, Tooltip("The force to apply to the rigidbody when the throttle is at 1")]
        private float _maxForce = 1000f;

        private void FixedUpdate()
        {
            // Move the rigidbody forward or backward with a force proportional to the throttle value
            _rigidbody.AddForce(transform.forward * _throttleValue * _maxForce * Time.fixedDeltaTime);
        }
    }
}

