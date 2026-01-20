using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncActiveController : NetworkBehaviour
{
    private readonly Dictionary<string, ActiveSyncTarget> markers = new();

    class ActiveSyncTarget
    {
        public string id;
        public GameObject go;
        public bool lastActive;
    }

    public ulong pcClientId;
    public override void OnNetworkSpawn()
    {
        var allMarkers = GetComponentsInChildren<ActiveSyncMarker>(true);
        foreach (var marker in allMarkers)
        {
            RegisterMarker(marker);
        }
    }

    // =========================================================
    public void RegisterMarker(ActiveSyncMarker marker)
    {
        if (!markers.ContainsKey(marker.Id))
        {
            markers[marker.Id] = new ActiveSyncTarget
            {
                id = marker.Id,
                go = marker.gameObject,
                lastActive = marker.gameObject.activeSelf
            };
        }
    }

    public void UnregisterMarker(ActiveSyncMarker marker)
    {
        if (markers.ContainsKey(marker.Id))
            markers.Remove(marker.Id);
    }

    // =========================================================
    public void NotifyActiveChange(ActiveSyncMarker marker, bool value)
    {
        if (!IsOwner)
        {
            if (markers.TryGetValue(marker.Id, out var t))
            {
                if (marker.gameObject.activeSelf != t.lastActive)
                {
                    marker.gameObject.SetActive(t.lastActive);
                }
            }
            return;
        }

        if (!IsSpawned) return;
        RequestSetActiveServerRpc(marker.Id, value);
    }

    // =========================================================
    [ServerRpc]
    void RequestSetActiveServerRpc(string id, bool value, ServerRpcParams rpcParams = default)
    {
        if (!IsSenderOwner(rpcParams))
            return;

        ApplyActive(id, value);

        //BroadcastToPC(id, value);
    }

    // =========================================================
    void ApplyActive(string id, bool value)
    {
        if (!markers.TryGetValue(id, out var t) || t.go == null)
            return;

        if (t.go.activeSelf == value) return;

        t.go.SetActive(value);
        t.lastActive = value;
    }


    void BroadcastToPC(string id, bool value)
    {
        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { pcClientId }
            }
        };

        ApplyActiveClientRpc(id, value, rpcParams);
    }

    // =========================================================
    [ClientRpc]
    void ApplyActiveClientRpc(string id, bool value, ClientRpcParams rpcParams = default)
    {
        if (IsOwner) return;
        ApplyActive(id, value);
    }

    // =========================================================
    bool IsSenderOwner(ServerRpcParams rpcParams)
    {
        var sender = rpcParams.Receive.SenderClientId;
        return sender == OwnerClientId;
    }
}
