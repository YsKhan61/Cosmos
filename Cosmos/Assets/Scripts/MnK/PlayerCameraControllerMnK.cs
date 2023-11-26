using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.MnK
{
    public class PlayerCameraControllerMnK : MonoBehaviour
    {
        [SerializeField] private InputActionReference _horizontalLookInputActionReference;
        [SerializeField] private InputActionReference _verticalLookInputActionReference;

        [SerializeField] private Transform _pivotTransform;

        [SerializeField] private float _sensitivity = 1f;

        [SerializeField, Tooltip("Invert Y Axis : 1 / -1")]
        private int _invertY = -1;

        private float _relativeAngleY = 0f;

        private Quaternion _horizontalRotation;
        private Quaternion _verticalRotation;
        private float _deltaY;
        private float _deltaTimeSensitivity;

        private void OnEnable()
        {
            _horizontalLookInputActionReference.action.Enable();
            _verticalLookInputActionReference.action.Enable();
        }

        private void Start()
        {
            
        }

        private void OnDisable()
        {
            _horizontalLookInputActionReference.action.Disable();
            _verticalLookInputActionReference.action.Disable();
        }

        private void Update()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            float xInput = _horizontalLookInputActionReference.action.ReadValue<float>();
            float yInput = _verticalLookInputActionReference.action.ReadValue<float>();

            Debug.Log($"xInput : {xInput} , yInput : {yInput}");

            _deltaTimeSensitivity = _sensitivity * Time.deltaTime;
            _deltaY = yInput * _deltaTimeSensitivity * _invertY;
            _relativeAngleY += _deltaY;

            if (_relativeAngleY > -90f && _relativeAngleY < 90f)
            {
                _verticalRotation = Quaternion.AngleAxis(_deltaY, _pivotTransform.right);
            }
            else
            {
                _relativeAngleY -= _deltaY;
                _verticalRotation = Quaternion.identity;
            }

            _horizontalRotation = Quaternion.AngleAxis(xInput * _deltaTimeSensitivity, _pivotTransform.up);

            transform.rotation = _horizontalRotation * _verticalRotation * transform.rotation;
        }        
    }
}

