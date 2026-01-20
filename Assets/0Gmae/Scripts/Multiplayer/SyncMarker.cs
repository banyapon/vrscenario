using Unity.Netcode;
using UnityEngine;

public class SyncMarker : MonoBehaviour
{
    [Header("Sync Identity")]
    [SerializeField] protected string id;

    protected NetworkObject networkObject;

    public string Id => id;

    protected virtual void Awake()
    {
        if (string.IsNullOrWhiteSpace(id))
            id = $"{gameObject.name}_{GetInstanceID()}";

        networkObject = GetComponentInParent<NetworkObject>();
    }

    public void Reset()
    {
        id = $"{gameObject.name}_{GetInstanceID()}";
    }
}
