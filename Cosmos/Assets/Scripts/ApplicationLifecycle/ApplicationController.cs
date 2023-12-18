using Cosmos.ConnectionManagement;
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
        private ConnectionManager _connectionManager;

        [SerializeField]
        private NetworkManager _networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_connectionManager);
            builder.RegisterComponent(_networkManager);
        }

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
    }

    
}

