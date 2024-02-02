using Cosmos.ApplicationLifecycle.Messages;
using Cosmos.ConnectionManagement;
using Cosmos.Infrastructure;
using Cosmos.Utilities;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// This is attached to Cosmos - scene game object, and it's referenced to LifeTimeScope of ServerCosmosState's AutoInjectGameObject.
    /// This will listen to the event raised by UIQuitPanelVR and will perform the same action as UIQuitPanel.
    /// </summary>
    public class SettingsVR : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO _onReturnToMenuEventRaised;
        [SerializeField] private VoidEventChannelSO _onQuitApplicationEventRaised;

        [Inject]
        private ConnectionManager _connectionManager;

        [Inject]
        private IPublisher<QuitApplicationMessage> _quitApplicationMessagePublisher;

        private void Awake()
        {
            _onQuitApplicationEventRaised.OnEventRaised += OnQuitApplicationEventRaised;
            _onReturnToMenuEventRaised.OnEventRaised += OnReturnToMenuEventRaised;
        }

        private void OnDestroy()
        {
            _onQuitApplicationEventRaised.OnEventRaised -= OnQuitApplicationEventRaised;
            _onReturnToMenuEventRaised.OnEventRaised -= OnReturnToMenuEventRaised;
        }

        private void OnQuitApplicationEventRaised()
        {
            _quitApplicationMessagePublisher.Publish(new QuitApplicationMessage());
        }

        private void OnReturnToMenuEventRaised()
        {
            _connectionManager?.RequestShutdown();
        }
    }
}
