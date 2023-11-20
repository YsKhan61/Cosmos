using Oculus.Interaction;
using UnityEngine;


namespace GalacticKittenVR.Spaceship
{
    /// <summary>
    /// Used to rotate a transform around a single axis based on the grabber position
    /// </summary>
    public class OneAxisRotateTransformer : MonoBehaviour, ITransformer
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
        private Axis _axis = Axis.Right;

        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {

        }

        public void UpdateTransform()
        {
            
        }

        public void EndTransform()
        {

        }

    }

}
