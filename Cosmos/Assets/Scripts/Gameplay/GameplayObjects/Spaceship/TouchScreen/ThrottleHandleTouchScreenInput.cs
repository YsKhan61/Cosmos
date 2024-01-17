using Cosmos.SpaceShip;
using UnityEngine;


namespace Cosmos.TouchScreen
{
    public class ThrottleHandleTouchScreenInput : MonoBehaviour
    {
        [SerializeField] private VariableJoystick _joystick;

        [SerializeField] private OneAxisRotateTransformer _oneAxisRotateTransformer;

        private void Update()
        {
            _oneAxisRotateTransformer.RotateInput = _joystick.Vertical;
        }
    }

}
