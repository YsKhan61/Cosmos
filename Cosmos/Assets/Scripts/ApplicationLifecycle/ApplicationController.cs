using Cosmos.ApplicationLifecycle.Messages;
using Cosmos.ConnectionManagement;
using Cosmos.Infrastructure;
using Cosmos.UnityServices;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;


namespace Cosmos.ApplicationLifecycle
{
    /// <summary>
    /// An entry point to the application, where we bind all the common dependencies to the root DI Scope
    /// </summary>
    public class ApplicationController : LifetimeScope
    {
        [SerializeField]
        private UpdateRunner _updateRunner;

        [SerializeField]
        private ConnectionManager _connectionManager;

        [SerializeField]
        private NetworkManager _networkManager;

        [SerializeField, Tooltip("Next scene to load")]
        private string _nextScene;

        private LocalLobby _localLobby;
        private LobbyServiceFacade _lobbyServiceFacade;

        private IDisposable _subscriptionsDisposable;

        private void Start()
        {
            _localLobby = Container.Resolve<LocalLobby>();
            _lobbyServiceFacade = Container.Resolve<LobbyServiceFacade>();

            ISubscriber<QuitApplicationMessage> quitApplicationMessageSubscriber =
                Container.Resolve<ISubscriber<QuitApplicationMessage>>();

            DisposableGroup subscribersDisposableGroup = new();
            subscribersDisposableGroup.Add(quitApplicationMessageSubscriber.Subscribe(QuitGame));
            _subscriptionsDisposable = subscribersDisposableGroup;

            Application.targetFrameRate = 72;
            Application.wantsToQuit += OnApplicationWantsToQuit;

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_updateRunner.gameObject);

            SceneManager.LoadScene(_nextScene);
        }

        

        protected override void OnDestroy()
        {
            if (_subscriptionsDisposable != null)
            {
                _subscriptionsDisposable.Dispose();
            }

            if(_lobbyServiceFacade != null)
            {
                _lobbyServiceFacade.EndTracking();
            }

            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponent(_updateRunner);
            builder.RegisterComponent(_connectionManager);
            builder.RegisterComponent(_networkManager);

            // the following singletons represent the local representations of the lobby that we're in and the user that we are.
            // they can persist longer than the lifetime of the UI in MainMenu, where we setup the lobby that we create or join.
            builder.Register<LocalLobby>(Lifetime.Singleton);
            builder.Register<LocalLobbyUser>(Lifetime.Singleton);

            builder.Register<ProfileManager>(Lifetime.Singleton);

            // these message channels are essential and persist for the lifetime of the lobby and relay services.
            // Registering as instance to prevent code stripping on IOS
            builder.RegisterInstance(new MessageChannel<QuitApplicationMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<UnityServiceErrorMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<ConnectStatus>()).AsImplementedInterfaces();

            // these message channels are essential and persist for the lifetime of the lobby and relay services.
            // they are networked so that the clients can subscribe to those messages that are published by the server.
            builder.RegisterComponent(new NetworkedMessageChannel<ConnectionEventMessage>()).AsImplementedInterfaces();

            // this message channel is essential and persists for the lifetime of the lobby and relay services.
            builder.RegisterInstance(new MessageChannel<ReconnectMessage>()).AsImplementedInterfaces();

            // buffered message channels hold the latest received message in buffer and pass to any new subscribers
            builder.RegisterInstance(new BufferedMessageChannel<LobbyListFetchedMessage>()).AsImplementedInterfaces();

            // all the lobby service stuff, bound here so that it persists through scene loads
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);  // a manager entity that allows us to do anonymous authentication with unity services

            // LobbyServiceFacade is registered as entrypoint because it wants a callback after container is built to do it's initialization
            builder.RegisterEntryPoint<LobbyServiceFacade>(Lifetime.Singleton).AsSelf();
        }

        private bool OnApplicationWantsToQuit()
        {
            Application.wantsToQuit -= OnApplicationWantsToQuit;

            bool canQuit = _localLobby != null && string.IsNullOrEmpty(_localLobby.LobbyID);
            if (!canQuit)
            {
                StartCoroutine(LeaveBeforeQuit());
            }

            return canQuit;
        }

        /// <summary>
        /// In builds, if we are in a lobby and try to send a Leave request on application quit,
        /// it won't go through if we're quitting on the same frame.
        /// So we need to delay just briefly to let the request happen (though we don't need to wait for the result)
        /// </summary>
        /// <returns></returns>
        private IEnumerator LeaveBeforeQuit()
        {
            // We want to quit anyways, so if anything happens while trying to leave the lobby, log the exception then carry on
            try
            {
                _lobbyServiceFacade.EndTracking();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            yield return null;
            Application.Quit();
        }

        private void QuitGame(QuitApplicationMessage message)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

