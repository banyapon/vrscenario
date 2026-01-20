using Unity.Netcode;
using UnityEngine;

public struct TransformState : INetworkSerializable
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
    }
}
