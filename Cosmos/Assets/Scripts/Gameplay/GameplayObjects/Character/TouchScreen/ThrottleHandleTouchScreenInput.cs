using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
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
