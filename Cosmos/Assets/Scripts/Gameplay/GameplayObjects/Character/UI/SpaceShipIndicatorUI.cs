using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character.UI
{
    /// <summary>
    /// Script to control the space ship indicator inside cockpit screen of spaceship model
    /// </summary>
    [ExecuteAlways]
    public class SpaceShipIndicatorUI : MonoBehaviour
    {
        [SerializeField] private Transform _spaceShipIndicatorTransform;

        private CancellationTokenSource cancellationTokenSource;

        private void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            // Start the UpdateTransform method
            RunUpdateTransform().Forget(); // Ignore the task to prevent warnings about not awaited Task
        }

        private async UniTaskVoid RunUpdateTransform()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                UpdateTransform();
                await UniTask.Yield(); // Yield to allow other tasks to execute
            }
        }

        private void OnDisable()
        {
            // Cancel and dispose the cancellationTokenSource
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private void OnDestroy()
        {
            // Cancel and dispose the cancellationTokenSource
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
            }
        }

        private void UpdateTransform()
        {
            _spaceShipIndicatorTransform.forward = Vector3.forward;
        }
    }
}

