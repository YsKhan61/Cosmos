using Cosmos.SpaceShip;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.MnK
{
    /// <summary>
    /// Used to rotate a transform around X Z axis based on MnK input
    /// </summary>
    public class XZAxesRotateTransformerMnK : MonoBehaviour, IControlHandle
    {
        private enum ControlState
        {
            AutoReset,
            Manual
        }

        private enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField] private InputActionReference _controlHandleInputActionReference;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The axis of the visual transform that need to be oriented to the grabber")]
        private Axis _axisOfVisualTransformToOrient = Axis.Up;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")] 
        private int _angleConstraint = 10;
        public int AngleConstraint => _angleConstraint;

        private Vector3 _localAxisToOrient;

        private Quaternion _initialVisualLocalRotation;

        private float _relativeAngleX;
        private float _relativeAngleZ;

        private ControlState _controlState;

        private void Awake()
        {
            _localAxisToOrient = Vector3.zero;
            _localAxisToOrient[(int)_axisOfVisualTransformToOrient] = 1;
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

            _relativeAngleX = 0;
            _relativeAngleZ = 0;
        }

        private void OnEnable()
        {
            _controlHandleInputActionReference.action.Enable();
            _controlHandleInputActionReference.action.started += OnControlHandleInputActionStarted;
            _controlHandleInputActionReference.action.canceled += OnControlHandleInputActionCanceled;
        }

        private void Update()
        {
            switch (_controlState) 
            {
                case ControlState.AutoReset:

                    AutoReset();

                    break;

                case ControlState.Manual:

                    ManualControl();

                    break;
            }
        }

        private void OnDisable()
        {
            _controlHandleInputActionReference.action.Disable();
            _controlHandleInputActionReference.action.started -= OnControlHandleInputActionStarted;
            _controlHandleInputActionReference.action.canceled -= OnControlHandleInputActionCanceled;
        }

        private void ManualControl()
        {
            Vector2 input = _controlHandleInputActionReference.action.ReadValue<Vector2>() * _sensitivity * Time.deltaTime;

            _relativeAngleX -= input.x;
            _relativeAngleZ -= input.y;

            _relativeAngleX = Mathf.Clamp(_relativeAngleX, -_angleConstraint, _angleConstraint);
            _relativeAngleZ = Mathf.Clamp(_relativeAngleZ, -_angleConstraint, _angleConstraint);

            Quaternion inputRotationInPivotSpace = Quaternion.Euler(_relativeAngleX, 0f, _relativeAngleZ);
            Quaternion inputRotationInWorldSpace = _pivotTransform.rotation * inputRotationInPivotSpace;
            _visualTransform.rotation = inputRotationInWorldSpace;
        }

        private void AutoReset()
        {
            _visualTransform.rotation = Quaternion.Slerp(
                    _visualTransform.rotation,
                    _pivotTransform.rotation * _initialVisualLocalRotation,
                    _sensitivity * Time.deltaTime);

            _relativeAngleX = _visualTransform.localEulerAngles.x;
            if (_relativeAngleX > 180)
                _relativeAngleX -= 360;


            _relativeAngleZ = _visualTransform.localEulerAngles.z;
            if (_relativeAngleZ > 180)
                _relativeAngleZ -= 360;
        }

        

        private void OnControlHandleInputActionStarted(InputAction.CallbackContext context)
        {
            _controlState = ControlState.Manual;
        }

        private void OnControlHandleInputActionCanceled(InputAction.CallbackContext context)
        {
            _controlState = ControlState.AutoReset;
        }
    }

}
