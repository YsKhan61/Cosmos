using Cosmos.Utilities;
using Oculus.Interaction;
using System.Collections;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Calculate the throttle input based on the grabber's position respect to the pivot
    /// </summary>
    public class CalculateThrottleInputVR : MonoBehaviour, ITransformer, IThrottleHandle
    {
        private enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField]
        private FloatDataSO _throttleInput;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;

        [SerializeField, Tooltip("The axis of rotation of the visual transform")]
        private Axis _axisOfRotation = Axis.Right;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")]
        private FloatDataSO _angleConstraint;

        [SerializeField]
        private bool _invert = false;

        private IGrabbable _grabbable;
        private Vector3 _axisOfRotationOfVisualTransformInLocalSpace;
        private Coroutine _resetOrientationRoutine;      

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
            _axisOfRotationOfVisualTransformInLocalSpace = Vector3.zero;
            _axisOfRotationOfVisualTransformInLocalSpace[(int)_axisOfRotation] = 1;
        }

        public void BeginTransform()
        {
            if (_resetOrientationRoutine != null)
            {
                StopCoroutine(_resetOrientationRoutine);
                _resetOrientationRoutine = null;
            }
        }

        public void UpdateTransform()
        {
            Vector3 vectorFromPivotToGrabberInWorldSpace =
                _grabbable.GrabPoints[0].position - _pivotTransform.position;

            
            Vector3 axisOfRotationInWorldSpace =            // axisOfRotationOfVisualTransformInWorldSpace
                _pivotTransform.TransformDirection(_axisOfRotationOfVisualTransformInLocalSpace);

            
            Vector3 projectedVector =                       // projectedVectorFromPivotToGrabberInWorldSpaceAlongPlaneWithNormalOfAxisToOrient
                Vector3.ProjectOnPlane(vectorFromPivotToGrabberInWorldSpace, axisOfRotationInWorldSpace);

            
            float signedAngle =                             // angleBetweenVectorFromPivotToGrabberInWorldSpaceAndPivotTransformYAxis
                Vector3.SignedAngle(projectedVector, _pivotTransform.up, axisOfRotationInWorldSpace) * (_invert ? -1 : 1);

            signedAngle = Mathf.Clamp(signedAngle, -_angleConstraint.value, _angleConstraint.value);

            _throttleInput.value = Mathf.Lerp(-1, 1, 
                Mathf.InverseLerp(-_angleConstraint.value, _angleConstraint.value, signedAngle));
        }

        public void EndTransform()
        {
            _resetOrientationRoutine ??= StartCoroutine(ResetOrientationRoutine());
        }

        IEnumerator ResetOrientationRoutine()
        {
            float time = 0f;
            float duration = 0.5f;

            while (time < duration)
            {
                time += Time.deltaTime;

                _throttleInput.value = Mathf.Lerp(_throttleInput.value, 0, time / duration);

                if (_throttleInput.value < 0.01f)
                {
                    _throttleInput.value = 0;
                    _resetOrientationRoutine = null;
                    yield break;
                }

                yield return null;
            }
        }

    }

}
