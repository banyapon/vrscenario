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


    PlayerData playerData = null;
    bool defaultActiveInitialize;
    List<DefaultActive> defaultActives = new List<DefaultActive>();

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
        VRManager manager = GetVRManager();
        if (manager != null) playerData = manager.GetComponent<PlayerData>();
        SetCamera(false);

        //if (!IsOwner && !(IsServer || IsHost))
        //{
        //    if (playerData == null) DisableChild();
        //    else if (playerData.IsInspector) DisableChild();
        //    else if (!playerData.IsInspector) playerData.OnInspector += DisableChild;
        //}

        if (VRNetworkController.Instance != null)//VR
        {
            if (VRNetworkController.Instance.inspector)
            {
                DisableChild();
                //player?.SetGravity(false);
                if (IsOwner)
                {
                    //DisableObjects();
                    //player?.SetGravity(false);
                    //player?.SetMove(false);
                    //player?.SetJump(false);
                    //player?.SetTeleportation(false);
                    //player?.SetTurn(false);
                }
                else
                {
                    //if (playerData.IsPlayer)
                    //{
                    //    InspectorSetup();
                    //}
                    //else
                    //{
                    //    playerData.OnPlayer += InspectorSetup;
                    //}
                }

                if (TrainingPlayerList.Instance.selectedClientId == OwnerClientId)
                {
                    InspectorSetup();
                }
            }
            else if (!IsOwner)
            {
                DisableChild();
            }
        }

        if (IsOwner || IsHost)
        {
            player = Player.Instance;
            player?.SetJump(false);
            player?.SetGravity(true);
            //player?.SetTeleportation(false);
            if (resetPlayerTransform) player?.Teleport(Vector3.zero, Vector3.zero, IsOwner);
            StartCount();
        }

        if ((IsServer || IsHost) && CCTVController.Instance != null)
        {
            SetCamera(true);
            if (manager != null)
            {
                manager.AppendAndSyncCameras(allCamera);
                manager.syncAudioList.Add(GetComponent<SyncAudioController>());
            }
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
    public void InspectorSetup()
    {
        ulong id = VRNetworkController.Instance.playerId;
        if (id != ulong.MaxValue && id != OwnerClientId)
        {
            print($"id != ulong.MaxValue: {id != ulong.MaxValue}");
            print($"id != OwnerClientId: {id != OwnerClientId}");
            return;
        }
        else
        {
            VRNetworkController.Instance.playerId = OwnerClientId;
        }
        ResetObjects();
        //playerData.OnDespawn += () =>
        //{
        //    print($"playerData {playerData.OwnerClientId} | Scenario {OwnerClientId}");
        //    VRNetworkController.Instance.Disconnect();
        //};

        foreach (var canvas in GetComponentsInChildren<Canvas>())
        {
            VRNetworkController.SetCanvasBlocked(canvas, true);
        }
    }
    void ResetObjects()
    {
        foreach (var item in defaultActives)
        {
            item.go.SetActive(item.value);
        }
    }

    private void DisableChild()
    {
        if (!defaultActiveInitialize)
        {
            defaultActiveInitialize = true;
            foreach (Transform child in transform)
            {
                DefaultActive defaultActive = new DefaultActive();
                defaultActive.value = child.gameObject.activeInHierarchy;
                defaultActive.go = child.gameObject;
                defaultActives.Add(defaultActive);
            }
        }

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
