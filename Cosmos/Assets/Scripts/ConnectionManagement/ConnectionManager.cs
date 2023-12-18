using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening to
    /// NetworkManager callbacks and other outside calls and redirecting them to the current ConnectionState.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        ConnectionState _currentState;

        [Inject]
        NetworkManager _networkManager;
        public NetworkManager NetworkManager => _networkManager;

        [SerializeField]
        int _reconnectAttempts = 2;

        public int MaxConnectedPlayers = 4;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }


    }
}


