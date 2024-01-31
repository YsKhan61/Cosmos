using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class ControlHandleTouchScreenInput : MonoBehaviour
    {
        [SerializeField] private VariableJoystick _joystick;

        [SerializeField] private XZAxesRotateTransformer _xzAxesRotateTransformer;

        /*private void Update()
        {
            _xzAxesRotateTransformer.RotateInput = _joystick.Direction;
        }*/
    }

}
