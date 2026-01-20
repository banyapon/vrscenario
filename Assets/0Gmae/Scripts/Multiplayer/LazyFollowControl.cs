using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(LazyFollow))]
[RequireComponent(typeof(TransformSyncMarker))]
public class LazyFollowControl : MonoBehaviour
{
    NetworkOwnershipContext ownership;
    LazyFollow lazyFollow;

    private void Awake()
    {
        ownership = GetComponentInParent<NetworkOwnershipContext>();
        if (ownership == null) return;

        lazyFollow = GetComponent<LazyFollow>();
        if (!ownership.IsOwnerClient)
        {
            lazyFollow.enabled = false;
            lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.None;
        }
    }

    //private void Reset()
    //{
    //    lazyFollow = GetComponent<LazyFollow>();
    //    lazyFollow.rotationFollowMode = LazyFollow.RotationFollowMode.LookAtWithWorldUp;
    //    GetComponent<TransformSyncMarker>().Reset();
    //}
}
