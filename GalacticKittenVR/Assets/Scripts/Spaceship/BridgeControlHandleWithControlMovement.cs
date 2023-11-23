using UnityEngine;


namespace GalacticKittenVR.Spaceship
{
    public class BridgeControlHandleWithControlMovement : MonoBehaviour
    {
        [SerializeField] 
        private AllAxisRotateTransformer _controlTransformer = null;

        [SerializeField]
        private ControlMovement _controlMovement = null;

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        [SerializeField, Tooltip("If the angle of this visual is less than deadzone limit, consider the value to be 0")]
        private float _deadZoneLimit = 0.1f;

        private void Update()
        {
            Vector3 v = _controlTransformer.PivotTransform.InverseTransformDirection(transform.up);

            /*float angleBetweenVectorFromPivotTransformToTransformUpInWorldSpaceAndPivotTransformUp = 
                Vector3.Angle(
                    vectorFromPivotTransformToTransformUpInWorldSpace, 
                    projectedVectorOfTransformUpOnPlaneWithNormalPivotTransformUp);*/

            _controlMovement.TorqueDirection =
                new Vector3(
                    v.x, 
                    0f, 
                    v.z).normalized;

            _controlMovement.TorqueMultiplier = 1f;

            /*_controlMovement.TorqueMultiplier = 
                Mathf.Lerp(
                    0f, 
                    1f, 
                    angleBetweenVectorFromPivotTransformToTransformUpInWorldSpaceAndPivotTransformUp / _controlTransformer.AngleConstraint);*/
        }
    }

}
