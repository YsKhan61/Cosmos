using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class ControlHandleInputValueFromKeyboard : MonoBehaviour
    {
        // [SerializeField] private InputActionReference _controlHandleInputActionReference;
        [SerializeField] private InputActionReference _pitchActionReference;
        [SerializeField] private InputActionReference _yawActionReference;
        [SerializeField] private InputActionReference _rollActionReference;

        [SerializeField] private Vector3DataSO _controlHandleInputData;

        [SerializeField]
        private bool _invertPitch = false;
        [SerializeField]
        private bool _invertYaw = false;
        [SerializeField]
        private bool _invertRoll = false;


        private void OnEnable()
        {
            // _controlHandleInputActionReference.action.Enable();
            _pitchActionReference.action.Enable();
            _yawActionReference.action.Enable();
            _rollActionReference.action.Enable();
        }

        private void Update()
        {
            // _controlHandleInputData.value = _controlHandleInputActionReference.action.ReadValue<Vector3>();
            _controlHandleInputData.value = new Vector3(
                            _pitchActionReference.action.ReadValue<float>() * (_invertPitch ? -1 : 1),
                            _yawActionReference.action.ReadValue<float>() * (_invertYaw ? -1 : 1),
                            _rollActionReference.action.ReadValue<float>() * (_invertRoll ? -1 : 1));
        }

        private void OnDisable()
        {
            // _controlHandleInputActionReference.action.Disable();
            _pitchActionReference.action.Disable();
            _yawActionReference.action.Disable();
            _rollActionReference.action.Disable();
        }
    }

}
