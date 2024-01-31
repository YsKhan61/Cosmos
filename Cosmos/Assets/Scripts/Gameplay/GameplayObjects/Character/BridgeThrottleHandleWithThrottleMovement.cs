
using Oculus.Interaction;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// This class is used to connect the throttle handle with the throttle movement
    /// attach this script to the throttle handle visual to get it's respective angle and set the throttle value
    /// </summary>
    public class BridgeThrottleHandleWithThrottleMovement : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IThrottleHandle))] 
        private Object _throttleHandleO = null;

        [SerializeField]
        private ThrottleMovement _throttleMovement = null;

        [SerializeField, Tooltip("1 or -1 : to invert the throttle visual angle value")]
        private int _invertMultiplier = 1;

        /*[SerializeField, Tooltip("If the angle of this visual is less than deadzone limit, consider the value to be 0")]
        private float _deadZoneLimit = 0.1f;*/

        private IThrottleHandle _throttleHandle = null;

        private void Start()
        {
            if (_throttleHandleO == null)
            {
                Debug.LogError("Throttle Handle is not set");
                enabled = false;
                return;
            }
            _throttleHandle = _throttleHandleO as IThrottleHandle;
        }

        private void Update()
        {
            float throttleVisualAngle = transform.localEulerAngles.z;                   // z is the axis of rotation

            if (throttleVisualAngle > 180f && throttleVisualAngle < 360f)
            {
                throttleVisualAngle = throttleVisualAngle - 360f;
            }

            // _throttleMovement.ThrottleValue = (throttleVisualAngle / (float)_throttleHandle.AngleConstraint) * _invertMultiplier;
                
        }
    }
}

