using UnityEngine;

public class ActiveSyncMarker : SyncMarker
{
    private SyncActiveController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInParent<SyncActiveController>();
        controller?.RegisterMarker(this);
    }

    private void OnDestroy()
    {
        controller?.UnregisterMarker(this);
    }

    private void OnEnable()
    {
        if (controller.IsServer || controller.IsHost) return;
        controller?.NotifyActiveChange(this, true);
    }

    private void OnDisable()
    {
        if (controller.IsServer || controller.IsHost) return;
        controller?.NotifyActiveChange(this, false);
    }
}
