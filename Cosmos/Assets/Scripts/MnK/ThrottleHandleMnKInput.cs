using Cosmos.SpaceShip;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.MnK
{
    public class ThrottleHandleMnKInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference _throttleInputActionReference;

        [SerializeField] private OneAxisRotateTransformer _oneAxisRotateTransformer;

        private void OnEnable()
        {
            _throttleInputActionReference.action.Enable();
        }

        private void Update()
        {
            _oneAxisRotateTransformer.RotateInput = _throttleInputActionReference.action.ReadValue<float>();
        }

        private void OnDisable()
        {
            _throttleInputActionReference.action.Disable();
        }
    }

}
