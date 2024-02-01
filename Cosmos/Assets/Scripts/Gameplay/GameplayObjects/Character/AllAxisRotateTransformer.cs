
using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Used to rotate a transform on pivot's X Y Z axis based on RotateInput;
    /// </summary>
    public class AllAxisRotateTransformer : MonoBehaviour, IControlHandle
    {
        [SerializeField, Tooltip("The data based on which the visual will rotate")]
        [FormerlySerializedAs("_rotateInput")]
        private Vector3DataSO _controlInput;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")] 
        private FloatDataSO _angleConstraint;

        [SerializeField]
        private bool _invertX = false;
        [SerializeField]
        private bool _invertY = false;
        [SerializeField]
        private bool _invertZ = false;

        private Quaternion _initialVisualLocalRotation;

        private float _relativeAngleX;
        private float _relativeAngleY;
        private float _relativeAngleZ;

        private void Awake()
        {
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

            _relativeAngleX = 0;
            _relativeAngleZ = 0;
        }

        /// <summary>
        /// We are taking the input directly from the CalculateControlInputValue and using it in ControlMovemnt,
        /// Hence, we can update the visual's orientation in LateUpdate
        /// </summary>
        private void LateUpdate()
        {
            if (_controlInput.value != Vector3.zero)
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
            Debug.Log($"_rotateInputValue: {_controlInput.value}");

            _relativeAngleX = Mathf.Lerp(-_angleConstraint.value, _angleConstraint.value, Mathf.InverseLerp(1, -1, _controlInput.value.x));
            _relativeAngleY = Mathf.Lerp(-_angleConstraint.value, _angleConstraint.value, Mathf.InverseLerp(1, -1, _controlInput.value.y));
            _relativeAngleZ = Mathf.Lerp(-_angleConstraint.value, _angleConstraint.value, Mathf.InverseLerp(1, -1, _controlInput.value.z));

            Debug.Log($"_relativeAngleX: {_relativeAngleX}, _relativeAngleY: {_relativeAngleY}, _relativeAngleZ: {_relativeAngleZ}");

            Quaternion inputRotationInPivotSpace = Quaternion.Euler(
                _relativeAngleZ * (_invertZ ? -1 : 1),
                _relativeAngleY * (_invertY ? -1 : 1),
                _relativeAngleX * (_invertX ? -1 : 1) 
            );

            _visualTransform.localRotation = Quaternion.Slerp(
                               _visualTransform.localRotation,
                                inputRotationInPivotSpace * _initialVisualLocalRotation,
                                5f* Time.deltaTime);
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
