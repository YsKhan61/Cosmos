using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class PlayerCameraControllerMnK : MonoBehaviour
    {
        [SerializeField] private InputActionReference _horizontalLookInputActionReference;
        [SerializeField] private InputActionReference _verticalLookInputActionReference;

        [SerializeField] private Transform _pivotTransform;

        [SerializeField] private float _sensitivity = 1f;

        [SerializeField, Tooltip("Invert Y Axis : 1 / -1")]
        private int _invertY = -1;

        private float _relativeAngleY;
        private float _relativeAngleX;

        private float _deltaTimeSensitivity;
        private float _xInput, _yInput;

        private void OnEnable()
        {
            _horizontalLookInputActionReference.action.Enable();
            _verticalLookInputActionReference.action.Enable();
        }

        private void Start()
        {
            transform.rotation = _pivotTransform.rotation;

            _xInput = 0f;
            _yInput = 0f;
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
            _deltaTimeSensitivity = _sensitivity * Time.deltaTime;
            _xInput = _horizontalLookInputActionReference.action.ReadValue<float>() * _deltaTimeSensitivity;
            _yInput = _verticalLookInputActionReference.action.ReadValue<float>() * _deltaTimeSensitivity * _invertY;

            _relativeAngleX += _xInput;
            _relativeAngleY = Mathf.Clamp(_relativeAngleY + _yInput, -90f, 90f);

            Vector3 localEulerAngles = new Vector3(_relativeAngleY, _relativeAngleX, 0f);

            transform.rotation = _pivotTransform.rotation * Quaternion.Euler(localEulerAngles);
        }
    }
}

