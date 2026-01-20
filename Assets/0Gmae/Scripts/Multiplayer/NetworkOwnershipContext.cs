using Unity.Netcode;
using UnityEngine;

public class NetworkOwnershipContext : NetworkBehaviour
{
    public bool IsOwnerClient => IsOwner;
    public bool IsServerClient => IsServer;
    public ulong OwnerId => OwnerClientId;
}
