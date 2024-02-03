using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Gameplay.Utilities
{
    

    /// <summary>
    /// Data representation of radars for all the clients.
    /// </summary>
    [CreateAssetMenu(fileName = "ServerRadarData", menuName = "ScriptableObjects/DataContainers/ServerRadarDataSO")]
    public sealed class ServerRadarDataSO : ScriptableObject
    {
        // public IReadOnlyList<RadarNetworkData> RadarDataList;
    }
}