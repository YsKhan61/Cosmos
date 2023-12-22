using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Utilities
{
    /// <summary>
    /// Wrapping FixedString so that if we want to change player name max size in the future, we only do it once here.
    /// </summary>
    public struct FixedPlayerName : INetworkSerializable
    {
        private FixedString32Bytes _name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _name);
        }

        public override string ToString()
        {
            return _name.Value.ToString();
        }

        public static implicit operator string(FixedPlayerName s) => s.ToString();
        public static implicit operator FixedPlayerName(string s) => new FixedPlayerName() { _name = new FixedString32Bytes(s) };
    }

    /// <summary>
    /// NetworkBehaviour containing only one NetworkVariableString which represents this object's name.
    /// </summary>
    public class NetworkNameState : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<FixedPlayerName> Name = new NetworkVariable<FixedPlayerName>();
    }
}

