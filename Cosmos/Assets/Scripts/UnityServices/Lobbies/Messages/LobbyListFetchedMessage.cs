using System.Collections.Generic;

namespace Cosmos.UnityServices.Lobbies
{
    public struct LobbyListFetchMessage
    {
        public readonly IReadOnlyList<LocalLobby> LocalLobbies;

        public LobbyListFetchMessage(List<LocalLobby> localLobbies)
        {
            LocalLobbies = localLobbies;
        }
    }
}