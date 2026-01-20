using UnityEngine;

public class TransformSyncMarker : SyncMarker
{
    private SyncTransformController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInParent<SyncTransformController>();
        controller?.RegisterMarker(this);
    }

    private void OnDestroy()
    {
        controller?.UnregisterMarker(this);
    }
}
