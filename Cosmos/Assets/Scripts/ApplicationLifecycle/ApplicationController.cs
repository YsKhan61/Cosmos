using Cosmos.ConnectionManagement;
using Cosmos.Infrastructure;
using Cosmos.UnityServices.Auth;
using Cosmos.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;


namespace Cosmos.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        [SerializeField]
        private UpdateRunner _updateRunner;

        [SerializeField]
        private ConnectionManager _connectionManager;

        [SerializeField]
        private NetworkManager _networkManager;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 72;
            SceneManager.LoadScene("MainMenu");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_updateRunner);
            builder.RegisterComponent(_connectionManager);
            builder.RegisterComponent(_networkManager);

            builder.Register<ProfileManager>(Lifetime.Singleton);

            // all the lobby service stuff, bound here so that it persists through scene loads
            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);  // a manager entity that allows us to do anonymous authentication with unity services
        }
    }

    
}

