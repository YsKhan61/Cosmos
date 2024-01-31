using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class ControlHandleInputValueFromKeyboard : MonoBehaviour
    {
        [SerializeField] private InputActionReference _controlHandleInputActionReference;

        [SerializeField] private Vector2DataSO _controlHandleInputData;

        private void OnEnable()
        {
            _controlHandleInputActionReference.action.Enable();
        }

        private void Update()
        {
            _controlHandleInputData.value = _controlHandleInputActionReference.action.ReadValue<Vector2>();
        }

        private void OnDisable()
        {
            _controlHandleInputActionReference.action.Disable();
        }
    }

}
