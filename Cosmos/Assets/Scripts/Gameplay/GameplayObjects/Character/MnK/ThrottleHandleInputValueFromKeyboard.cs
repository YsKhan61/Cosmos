using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class ThrottleHandleInputValueFromKeyboard : MonoBehaviour
    {
        [SerializeField] private InputActionReference _throttleInputActionReference;

        [SerializeField] private FloatDataSO _throttleHandleInputData;

        private void OnEnable()
        {
            _throttleInputActionReference.action.Enable();
        }

        private void Update()
        {
            _throttleHandleInputData.value = _throttleInputActionReference.action.ReadValue<float>();
        }

        private void OnDisable()
        {
            _throttleInputActionReference.action.Disable();
        }
    }

}
