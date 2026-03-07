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
        controller?.NotifyActiveChange(this, true);
    }

    private void OnDisable()
    {
        controller?.NotifyActiveChange(this, false);
    }
}
