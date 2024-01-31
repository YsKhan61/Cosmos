
using Cosmos.Utilities;
using Oculus.Interaction;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Calculates the throttle value from the throttle handle visual's Z angle
    /// </summary>
    public class CalculateThrottleMovementInputValue : MonoBehaviour
    {
        [SerializeField, Tooltip("This value will be used in ThrottleMovement script to move the spaceship")] 
        private FloatDataSO _throttleMovementInput = null;

        [SerializeField, Tooltip("The max angle a throttle handle can move in pos or neg")] 
        private FloatDataSO _throttleAngleConstraint;

        [SerializeField, Tooltip("1 or -1 : to invert the throttle visual angle value")]
        private int _invertMultiplier = 1;

        private void Update()
        {
            float throttleVisualAngle = transform.localEulerAngles.z;                   // z is the axis of rotation

            if (throttleVisualAngle > 180f && throttleVisualAngle < 360f)
            {
                throttleVisualAngle = throttleVisualAngle - 360f;
            }

            _throttleMovementInput.value = (throttleVisualAngle / _throttleAngleConstraint.value) * _invertMultiplier; 
        }
    }
}

