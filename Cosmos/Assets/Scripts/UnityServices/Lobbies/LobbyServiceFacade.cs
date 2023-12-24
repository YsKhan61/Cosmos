using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using VContainer.Unity;

namespace Cosmos.UnityServices.Lobbies
{
    public class LobbyServiceFacade : IDisposable, IStartable
    {

        public void Dispose()
        {

        }

        public void Start()
        {

        }

        /// <summary>
        /// Attempt to create a new lobby and then join it.
        /// </summary>
        /// <param name="lobbyName"></param>
        /// <param name="maxPlayers"></param>
        /// <param name="isPrivate"></param>
        /// <returns></returns>
        public async Task<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxPlayers, bool isPrivate)
        {
            return (false, null);
        }

        public void SetRemoteLobby(Lobby lobby)
        {
            
        }

        public Task RetrieveAndPublishLobbyListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(bool Success, Lobby Lobby)> TryJoinLobbyAsync(object value, string lobbyCode)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Success, Lobby Lobby)> TryQuickJoinLobbyAsync()
        {
            throw new NotImplementedException();
        }
    }
}