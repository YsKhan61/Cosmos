using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Used to rotate a visual transform around pivot's forward axis based on a (1 to -1) value;
    /// </summary>
    public class OneAxisRotateTransformer : MonoBehaviour, IThrottleHandle
    {
        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")]
        private float _angleConstraint = 10;
        public float AngleConstraint => _angleConstraint;

        [SerializeField, Tooltip(" Assign 1 or -1")]
        private int _invertThrottleValue = 1;

        private float _rotateInput;
        public float RotateInput { set => _rotateInput = value; }

        private float _relativeAngleZ;
        private Quaternion _initialVisualLocalRotation;

        private void Awake()
        {
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

            _relativeAngleZ = 0;
        }

        private void Update()
        {
            if (_rotateInput != 0)
            {
                ManualThrottleControl();
            }
            else
            {
                AutoThrottleReset();
            }
        }

        private void ManualThrottleControl()
        {
            _relativeAngleZ += _sensitivity * _invertThrottleValue * _rotateInput * Time.deltaTime;
            _relativeAngleZ = Mathf.Clamp(_relativeAngleZ, -_angleConstraint, _angleConstraint);

            Debug.Log("Relative Angle Z: " + _relativeAngleZ);

            Quaternion inputRotationInPivotSpace = Quaternion.Euler(0f, 0f, _relativeAngleZ);
            Quaternion inputRotationInWorldSpace = _pivotTransform.rotation * inputRotationInPivotSpace;
            _visualTransform.rotation = inputRotationInWorldSpace;
        }

        private void AutoThrottleReset()
        {
            _visualTransform.rotation = 
                Quaternion.Slerp(_visualTransform.rotation, _pivotTransform.rotation * _initialVisualLocalRotation, _sensitivity * Time.deltaTime);

            _relativeAngleZ = _visualTransform.localEulerAngles.z;
            if (_relativeAngleZ > 180)
                _relativeAngleZ -= 360;
        }
    }

}
