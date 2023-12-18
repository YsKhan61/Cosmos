using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// A temporary animatino script that rotates the image during loading.
    /// </summary>
    public class ConnectionAnimation : MonoBehaviour
    {
        [SerializeField]
        private float _rotationSpeed = -50f;

        private void Update()
        {
            transform.Rotate(0, 0, _rotationSpeed * Mathf.PI * Time.deltaTime);
        }
    }

}