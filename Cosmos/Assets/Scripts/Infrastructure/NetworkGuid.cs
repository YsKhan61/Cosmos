using System;
using Unity.Netcode;

namespace Cosmos.Infrastructure
{
    public struct NetworkGuid : INetworkSerializeByMemcpy
    {
        public ulong FirstHalf;
        public ulong SecondHalf;
    }

    public static class NetworkGuidExtensions
    {
        public static NetworkGuid ToNetworkGuid(this Guid id)
        {
            NetworkGuid networkGuid = new NetworkGuid();
            networkGuid.FirstHalf = BitConverter.ToUInt64(id.ToByteArray(), 0);
            networkGuid.SecondHalf = BitConverter.ToUInt64(id.ToByteArray(), 8);
            return networkGuid;
        }

        public static Guid ToGuid(this NetworkGuid networkGuid)
        {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(networkGuid.FirstHalf), 0, bytes, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(networkGuid.SecondHalf), 0, bytes, 8, 8);
            return new Guid(bytes);
        }
    }
}