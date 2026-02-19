using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Scenario : NetworkBehaviour
{
    [SerializeField] private bool resetPlayerTransform = true;
    public float timeUsed;
    private bool isCounting = false;
    [SerializeField] private Button destroyBtn;
    [SerializeField] private List<Camera> allCamera = new List<Camera>();
    List<GameObject> grabObjects = new List<GameObject>();
    Player player;

    private void Awake()
    {
        if (destroyBtn) destroyBtn.onClick.AddListener(RequestDestroy);

        foreach (var grab in GetComponentsInChildren<XRGrabInteractable>(true))
        {
            grabObjects.Add(grab.gameObject);
        }
    }

    private void Update()
    {
        if (!isCounting) return;
        timeUsed += Time.deltaTime;
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
            player = Player.Instance;
            player?.SetJump(false);
            //player?.SetTeleportation(false);
            if (resetPlayerTransform) player?.Teleport(Vector3.zero, Vector3.zero, IsOwner);
            StartCount();
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
        ClearGrabObject();
        GetVRManager()?.OpenBoardUI();
        player?.SetJump(true);
        StopCount();
        //player?.SetTeleportation(true);
        CCTVController cctv = CCTVController.Instance;
        if ((IsServer || IsHost) && cctv != null)
        {
            cctv.SetCameraIndex(OwnerClientId, 0);
            cctv.SetViewerCategory(OwnerClientId, CCTVCategory.Lobby);
            GetVRManager()?.AppendAndSyncCameras(null);
            print($"Scenario OnDespawn {OwnerClientId}");
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
    public void StartCount()
    {
        isCounting = true;
    }

    public void StopCount()
    {
        isCounting = false;
    }

    public void ResetCount()
    {
        timeUsed = 0f;
    }
    public void RestartCount()
    {
        timeUsed = 0f;
        isCounting = true;
    }
}
