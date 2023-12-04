using Cosmos.SpaceShip;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.SpaceShip
{
    /// <summary>
    /// Used to rotate a transform on pivot's X Z axis based on RotateInput;
    /// </summary>
    public class XZAxesRotateTransformer : MonoBehaviour, IControlHandle
    {
        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")] 
        private int _angleConstraint = 10;
        public int AngleConstraint => _angleConstraint;

        private Quaternion _initialVisualLocalRotation;

        private float _relativeAngleX;
        private float _relativeAngleZ;

        private Vector2 _rotateInput;
        public Vector2 RotateInput { set => _rotateInput = value; }

        private void Awake()
        {
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

            _relativeAngleX = 0;
            _relativeAngleZ = 0;
        }

        private void Update()
        {
            if (_rotateInput != Vector2.zero)
            {
                ManualControl();
            }
            else
            {
                AutoReset();
            }
        }

        private void ManualControl()
        {
            _relativeAngleX -= _rotateInput.x;
            _relativeAngleZ -= _rotateInput.y;

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
    }

}
