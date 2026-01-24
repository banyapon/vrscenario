using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Boy;

public class Scenario : NetworkBehaviour
{
    [SerializeField] private Button destroyBtn;
    [SerializeField] private List<Camera> allCamera = new List<Camera>();
    List<GameObject> grabObjects = new List<GameObject>();

    private void Awake()
    {
        if (destroyBtn) destroyBtn.onClick.AddListener(RequestDestroy);

        foreach (var grab in GetComponentsInChildren<XRGrabInteractable>(true))
        {
            grabObjects.Add(grab.gameObject);
        }
    }

    public void RequestDestroy()
    {
        if (!IsOwner) return;
        RequestDestroyServerRpc();
    }

    [ServerRpc]
    void RequestDestroyServerRpc()
    {
        NetworkObject.Despawn(true);
    }
    public override void OnNetworkSpawn()
    {
        SetCamera(false);

        if (!IsOwner && !(IsServer || IsHost))
        {
            DisableChild();
        }

        if (IsOwner || IsHost)
        {
            Player player = Player.Instance;
            if (player) player.Teleport(Vector3.zero, Vector3.zero);
        }

        if ((IsServer || IsHost) && CCTVController.Instance != null)
        {
            SetCamera(true);
            GetVRManager()?.AppendAndSyncCameras(allCamera);
        }

    }
    public override void OnNetworkDespawn()
    {
        OnDespawn();
    }

    void OnDespawn()
    {
        //print("OnDespawn");
        ClearGrabObject();
        GetVRManager()?.OpenBoardUI();
        CCTVController cctv = CCTVController.Instance;
        if ((IsServer || IsHost) && cctv != null)
        {
            cctv.SetCameraIndex(OwnerClientId, 0);
        }
    }

    VRManager GetVRManager()
    {
        VRManager[] vRManagers = FindObjectsByType<VRManager>(FindObjectsSortMode.None);
        foreach (var vrManager in vRManagers)
        {
            if (vrManager.OwnerClientId == OwnerClientId) return vrManager;
        }

        return null;
    }

    void ClearGrabObject()
    {
        foreach (var grabObj in grabObjects)
        {
            if (grabObj == null) continue;
            Destroy(grabObj);
        }
    }

    private void DisableChild()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void SetCamera(bool value)
    {
        foreach (var camera in allCamera)
        {
            camera.enabled = value;
        }
    }
}
