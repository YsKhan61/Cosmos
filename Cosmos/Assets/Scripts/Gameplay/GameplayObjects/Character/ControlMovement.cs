using Cosmos.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Takes a Vector3 as input axis, for providing the torque to the rigidbody
    /// </summary>
    public class ControlMovement : MonoBehaviour
    {
        [SerializeField, Tooltip("The orientation along which to apply torque")]
        private Vector3DataSO _controlMovementDirectionInput;

        [SerializeField, Tooltip("The factor by which the torque is multiplied")]
        private float _torqueMultiplier = 1f;

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
            _rigidbody.AddRelativeTorque(_controlMovementDirectionInput.value * _torqueMultiplier * _maxTorqueToInput * Time.fixedDeltaTime);

            if(_controlMovementDirectionInput.value == Vector3.zero)
                _rigidbody.angularVelocity = Vector3.Slerp(_rigidbody.angularVelocity, Vector3.zero, _damping * Time.fixedDeltaTime);
        }
    }

}
