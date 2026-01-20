using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncTransformController : NetworkBehaviour
{
    class SyncTarget
    {
        public string id;
        public Transform transform;

        public Vector3 lastSentPos;
        public Quaternion lastSentRot;

        public Vector3 targetPos;
        public Quaternion targetRot;
    }

    [Header("Sync Settings")]
    public float sendRate = 20f;
    public float positionThreshold = 0.001f;
    public float rotationThreshold = 0.5f;

    [Header("Interpolation (Non-Owner)")]
    public float smoothSpeed = 15f;

    private float timer;

    // <==== Dictionary instead of List
    private readonly Dictionary<string, SyncTarget> targets = new();

    public ulong pcClientId;

    // =========================================================
    public override void OnNetworkSpawn()
    {
        var allMarkers = GetComponentsInChildren<TransformSyncMarker>(true);
        foreach (var marker in allMarkers)
        {
            RegisterMarker(marker);
        }

        InitTargets();
    }

    // =========================================================
    public void RegisterMarker(TransformSyncMarker marker)
    {
        if (marker == null) return;

        targets[marker.Id] = new SyncTarget
        {
            id = marker.Id,
            transform = marker.transform,
            lastSentPos = marker.transform.localPosition,
            lastSentRot = marker.transform.localRotation,
            targetPos = marker.transform.localPosition,
            targetRot = marker.transform.localRotation
        };
    }

    public void UnregisterMarker(TransformSyncMarker marker)
    {
        if (marker != null)
            targets.Remove(marker.Id);
    }

    // =========================================================
    void InitTargets()
    {
        foreach (var kv in targets)
        {
            var t = kv.Value;
            if (t.transform == null) continue;

            t.lastSentPos = t.transform.localPosition;
            t.lastSentRot = t.transform.localRotation;
            t.targetPos = t.transform.localPosition;
            t.targetRot = t.transform.localRotation;
        }
    }

    // =========================================================
    void Update()
    {
        if (!IsOwner || !IsClient) return;

        timer += Time.deltaTime;
        if (timer < 1f / sendRate) return;
        timer = 0f;

        var changed = CollectChangedStates();
        if (changed.Count > 0)
            SendTransformBatchServerRpc(changed.ToArray());
    }

    void LateUpdate()
    {
        if (IsOwner) return;

        foreach (var kv in targets)
        {
            var t = kv.Value;
            if (t.transform == null) continue;

            t.transform.localPosition = Vector3.Lerp(
                t.transform.localPosition,
                t.targetPos,
                Time.deltaTime * smoothSpeed
            );

            t.transform.localRotation = Quaternion.Slerp(
                t.transform.localRotation,
                t.targetRot,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    // =========================================================
    List<TransformState> CollectChangedStates()
    {
        List<TransformState> result = new();

        foreach (var kv in targets)
        {
            var t = kv.Value;
            if (t.transform == null) continue;

            if (Vector3.Distance(t.lastSentPos, t.transform.localPosition) > positionThreshold ||
               Quaternion.Angle(t.lastSentRot, t.transform.localRotation) > rotationThreshold)
            {
                result.Add(new TransformState
                {
                    id = t.id,
                    position = t.transform.localPosition,
                    rotation = t.transform.localRotation
                });

                t.lastSentPos = t.transform.localPosition;
                t.lastSentRot = t.transform.localRotation;
            }
        }

        return result;
    }

    // =========================================================
    [ServerRpc]
    void SendTransformBatchServerRpc(TransformState[] states)
    {
        ApplyStates(states);
        //BroadcastToPC(states);
    }

    void BroadcastToPC(TransformState[] states)
    {
        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { pcClientId }
            }
        };

        ApplyTransformClientRpc(states, rpcParams);
    }

    // =========================================================
    [ClientRpc]
    void ApplyTransformClientRpc(TransformState[] states, ClientRpcParams rpcParams = default)
    {
        if (IsOwner) return;
        ApplyStates(states);
    }

    void ApplyStates(TransformState[] states)
    {
        foreach (var s in states)
        {
            if (!targets.TryGetValue(s.id, out var t) || t.transform == null)
                continue;

            t.targetPos = s.position;
            t.targetRot = s.rotation;
        }
    }
}
