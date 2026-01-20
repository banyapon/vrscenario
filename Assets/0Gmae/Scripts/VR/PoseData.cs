using Unity.Netcode;
using UnityEngine;

public struct PoseData : INetworkSerializable
{
    public Vector3 position;
    public Quaternion rotation;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
    }
}
