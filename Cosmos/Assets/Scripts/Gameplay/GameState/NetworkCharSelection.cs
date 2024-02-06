using Cosmos.Gameplay.Configuration;
using Cosmos.Utilities;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameState
{
    /// <summary>
    /// Common data and RPCs for the CharSelect state.
    /// </summary>
    public class NetworkCharSelection : NetworkBehaviour
    {
        public enum SeatState : byte
        {
            Inactive,
            Active,
            LockedIn
        }

        public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
        {
            public ulong ClientId;

            private FixedPlayerName m_PlayerName; // I'm sad also there's no 256Bytes fixed list :(

            public int PlayerNumber; // this player's assigned "P#". (0=P1, 1=P2, etc.)
            public int SeatIdx; // the latest seat they were in. -1 means none
            public float LastChangeTime;

            public SeatState SeatState;

            public LobbyPlayerState(ulong clientId, string name, int playerNumber, SeatState state, int seatIdx = -1, float lastChangeTime = 0)
            {
                ClientId = clientId;
                PlayerNumber = playerNumber;
                SeatState = state;
                SeatIdx = seatIdx;
                LastChangeTime = lastChangeTime;
                m_PlayerName = new FixedPlayerName();

                PlayerName = name;
            }

            public string PlayerName
            {
                get => m_PlayerName;
                private set => m_PlayerName = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref m_PlayerName);
                serializer.SerializeValue(ref PlayerNumber);
                serializer.SerializeValue(ref SeatState);
                serializer.SerializeValue(ref SeatIdx);
                serializer.SerializeValue(ref LastChangeTime);
            }

            public readonly bool Equals(LobbyPlayerState other)
            {
                return ClientId == other.ClientId &&
                       m_PlayerName.Equals(other.m_PlayerName) &&
                       PlayerNumber == other.PlayerNumber &&
                       SeatIdx == other.SeatIdx &&
                       LastChangeTime.Equals(other.LastChangeTime) &&
                       SeatState == other.SeatState;
            }
        }

        private NetworkList<LobbyPlayerState> _lobbyPlayers;

        public AvatarSO[] AvatarConfigurations;

        private void Awake()
        {
            _lobbyPlayers = new NetworkList<LobbyPlayerState>();
        }

        /// <summary>
        /// Current state of all players in the lobby.
        /// </summary>
        public NetworkList<LobbyPlayerState> LobbyPlayers => _lobbyPlayers;

        /// <summary>
        /// When this becomes true, the lobby is closed and in process of terminating (switching to gameplay).
        /// </summary>
        public NetworkVariable<bool> IsLobbyClosed { get; } = new NetworkVariable<bool>(false);

        /// <summary>
        /// Server notification when a client requests a different lobby-seat, or locks in their seat choice
        /// </summary>
        public event Action<ulong, int, bool> OnClientChangedSeat;

        /// <summary>
        /// RPC to notify the server that a client has chosen a seat.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ChangeSeatServerRpc(ulong clientId, int seatIdx, bool lockedIn)
        {
            OnClientChangedSeat?.Invoke(clientId, seatIdx, lockedIn);
        }
    }
}
