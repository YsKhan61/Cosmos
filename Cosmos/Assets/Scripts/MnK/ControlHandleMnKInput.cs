using Cosmos.SpaceShip;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.MnK
{
    public class ControlHandleMnKInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference _controlHandleInputActionReference;

        [SerializeField] private XZAxesRotateTransformer _xzAxesRotateTransformer;

        private void OnEnable()
        {
            _controlHandleInputActionReference.action.Enable();
        }

        private void Update()
        {
            _xzAxesRotateTransformer.RotateInput = _controlHandleInputActionReference.action.ReadValue<Vector2>();
        }

        private void OnDisable()
        {
            _controlHandleInputActionReference.action.Disable();
        }
    }

}
