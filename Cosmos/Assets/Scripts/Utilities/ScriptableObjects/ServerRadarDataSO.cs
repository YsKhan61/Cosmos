using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Gameplay.Utilities
{
    [Serializable]
    public struct RadarNetworkData : INetworkSerializable, IEquatable<RadarNetworkData>
    {
        public ulong ClientId;                  // ClientId of the player over the network.
        public Vector3 AvatarPosition;          // Position of avatar in world space that will be used to calculate the position on the radar.
        public Color ImageColor;                // Color of the avatar on the radar.

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AvatarPosition);
            serializer.SerializeValue(ref ImageColor);
        }

        public bool Equals(RadarNetworkData other)
        {
            return ClientId == other.ClientId;
        }
    }

    /// <summary>
    /// Data representation of radars for all the clients.
    /// </summary>
    [CreateAssetMenu(fileName = "ServerRadarData", menuName = "ScriptableObjects/DataContainers/ServerRadarDataSO")]
    public sealed class ServerRadarDataSO : ScriptableObject
    {
        public IReadOnlyList<RadarNetworkData> RadarDataList;
    }
}