using Cosmos.SpaceShip;
using Oculus.Interaction;
using System.Collections;
using UnityEngine;


namespace Cosmos.VR
{
    /// <summary>
    /// Used to rotate a transform around a single axis based on the grabber position
    /// </summary>
    public class OneAxisRotateTransformerVR : MonoBehaviour, ITransformer, IThrottleHandle
    {
        private const int _ANGLE_LIMIT = 10;

        private enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The axis of rotation of the visual transform")]
        private Axis _axisOfRotation = Axis.Right;

        [SerializeField, Tooltip("The axis of visual transform that need to be oriented to the grabber")]
        private Axis _axisToOrient = Axis.Up;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")]
        private float _angleConstraint = 10;
        public float AngleConstraint => _angleConstraint;

        private IGrabbable _grabbable;
        private Vector3 _localAxisOfRotationOfPivotTransform;
        private Vector3 _localAxisOfVisualTransformToOrient;

        private Quaternion _initialVisualLocalRotation;
        private Coroutine _resetOrientationRoutine;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
            _localAxisOfRotationOfPivotTransform = Vector3.zero;
            _localAxisOfRotationOfPivotTransform[(int)_axisOfRotation] = 1;
            _localAxisOfVisualTransformToOrient = Vector3.zero;
            _localAxisOfVisualTransformToOrient[(int)_axisToOrient] = 1;
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;
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

            Vector3 axisOfRotation = _pivotTransform.TransformDirection(_localAxisOfRotationOfPivotTransform);
            Vector3 axisToOrient = _visualTransform.TransformDirection(_localAxisOfVisualTransformToOrient);

            Vector3 projectedVectorFromPivotToGrabberInWorldSpaceAlongPlaneWithNormalOfAxisToOrient =
                Vector3.ProjectOnPlane(vectorFromPivotToGrabberInWorldSpace, axisOfRotation);

            float angleBetweenVectorFromPivotToGrabberInWorldSpaceAndPivotTransformYAxis =
                Vector3.Angle(projectedVectorFromPivotToGrabberInWorldSpaceAlongPlaneWithNormalOfAxisToOrient, _pivotTransform.up);

            if (angleBetweenVectorFromPivotToGrabberInWorldSpaceAndPivotTransformYAxis > _angleConstraint)
            {
                return;
            }

            _visualTransform.rotation =
                Quaternion.FromToRotation(
                    axisToOrient,
                    projectedVectorFromPivotToGrabberInWorldSpaceAlongPlaneWithNormalOfAxisToOrient) * _visualTransform.rotation;
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
                Quaternion startLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

                _visualTransform.rotation = 
                    _pivotTransform.rotation * Quaternion.Slerp(startLocalRotation, _initialVisualLocalRotation, time / duration);

                yield return null;
            }
        }

    }

}
