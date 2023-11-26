using Cosmos.SpaceShip;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.MnK
{
    /// <summary>
    /// Used to rotate a transform around a single axis based on the MnK input
    /// </summary>
    public class OneAxisRotateTransformerMnK : MonoBehaviour, IThrottleHandle
    {
        private const int _ANGLE_LIMIT = 10;

        private enum Direction : int
        {
            Idle = 0,
            Forward = -1,
            Backward = 1
        }

        private enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField] private InputActionReference _throttleForwardInputActionReference;
        [SerializeField] private InputActionReference _throttleBackwardInputActionReference;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The axis of rotation of the visual transform")]
        private Axis _axisOfRotation = Axis.Right;

        [SerializeField, Tooltip("The axis of visual transform that is used to check angle of rotation respect to same axis of pivot")]
        private Axis _axisToOrient = Axis.Up;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")]
        private float _angleConstraint = 10;
        public float AngleConstraint => _angleConstraint;

        private Direction _directionToRotateThrottle = Direction.Idle;
        private Vector3 _axisOfRotationOfVisualTransformInLocalSpace;
        private Vector3 _axisOfOrientationOfVisualTransformInLocalSpace;

        private void Awake()
        {
            _axisOfRotationOfVisualTransformInLocalSpace = Vector3.zero;
            _axisOfRotationOfVisualTransformInLocalSpace[(int)_axisOfRotation] = 1;

            _axisOfOrientationOfVisualTransformInLocalSpace = Vector3.zero;
            _axisOfOrientationOfVisualTransformInLocalSpace[(int)_axisToOrient] = 1;
        }

        private void OnEnable()
        {
            _throttleForwardInputActionReference.action.started += OnThrottleForwardInputStarted;
            _throttleBackwardInputActionReference.action.started += OnThrottleBackwardInputStarted;

            _throttleForwardInputActionReference.action.canceled += OnThrottleForwardInputCanceled;
            _throttleBackwardInputActionReference.action.canceled += OnThrottleBackwardInputCanceled;

            _throttleForwardInputActionReference.action.Enable();
            _throttleBackwardInputActionReference.action.Enable();
        }

        private void Update()
        {
            switch (_directionToRotateThrottle)
            {
                case Direction.Idle:

                    AutoThrottleReset();

                    break;

                case Direction.Forward:
                case Direction.Backward:

                    ManualThrottleControl();

                    break;

                default:
                    break;
            }
        }

        private void OnDisable()
        {
            _throttleForwardInputActionReference.action.started -= OnThrottleForwardInputStarted;
            _throttleBackwardInputActionReference.action.started -= OnThrottleBackwardInputStarted;

            _throttleForwardInputActionReference.action.canceled -= OnThrottleForwardInputCanceled;
            _throttleBackwardInputActionReference.action.canceled -= OnThrottleBackwardInputCanceled;

            _throttleForwardInputActionReference.action.Disable();
            _throttleBackwardInputActionReference.action.Disable();
        }

        private void ManualThrottleControl()
        {
            float signedAngle = Vector3.SignedAngle(
                _pivotTransform.TransformDirection(_axisOfOrientationOfVisualTransformInLocalSpace),
                _visualTransform.TransformDirection(_axisOfOrientationOfVisualTransformInLocalSpace),
                _visualTransform.TransformDirection(_axisOfRotationOfVisualTransformInLocalSpace));

            bool allowToRotateVisual()
            {
                if (Mathf.Abs(signedAngle) > _angleConstraint)
                {
                    if (signedAngle < 0 && _directionToRotateThrottle == Direction.Forward)
                        return false;

                    if (signedAngle > 0 && _directionToRotateThrottle == Direction.Backward)
                        return false;
                }

                return true;
            }

            if (!allowToRotateVisual())
                return;

            Quaternion rotation =
                Quaternion.AngleAxis(
                    _sensitivity * (int)_directionToRotateThrottle * Time.deltaTime,
                    _visualTransform.TransformDirection(_axisOfRotationOfVisualTransformInLocalSpace));

            _visualTransform.rotation = rotation * _visualTransform.rotation;
        }

        private void AutoThrottleReset()
        {
            Quaternion fromToRotation = Quaternion.FromToRotation(
                _visualTransform.TransformDirection(_axisOfOrientationOfVisualTransformInLocalSpace),
                _pivotTransform.TransformDirection(_axisOfOrientationOfVisualTransformInLocalSpace));

            _visualTransform.rotation = Quaternion.Slerp(
                _visualTransform.rotation, 
                fromToRotation * _visualTransform.rotation, 
                Time.deltaTime * _sensitivity);
        }

        private void OnThrottleForwardInputStarted(InputAction.CallbackContext context)
        {
            _directionToRotateThrottle = Direction.Forward;
        }

        private void OnThrottleForwardInputCanceled(InputAction.CallbackContext context)
        {
            _directionToRotateThrottle = Direction.Idle;
        }

        private void OnThrottleBackwardInputStarted(InputAction.CallbackContext context)
        {
            _directionToRotateThrottle = Direction.Backward;
        }

        private void OnThrottleBackwardInputCanceled(InputAction.CallbackContext context)
        {
            _directionToRotateThrottle = Direction.Idle;
        }
    }

}
