using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class VRManager : NetworkBehaviour
{
    public bool isMute = true;
    //[Header("Camera")]
    [SerializeField] private GameObject playerMock;
    [SerializeField] private GameObject headMock;
    [SerializeField] private List<Camera> allCamera = new List<Camera>();
    public List<SyncAudioController> syncAudioList = new List<SyncAudioController>();

    [Header("Scenario")]
    public GameObject boardUI;
    public GameObject environment;
    public GameObject lobbyRoom;
    public Button startBtn;
    public Button disconnectBtn;
    [SerializeField] private ScenarioConfig[] scenarioConfigs;

    [SerializeField] private ScenarioConfig currentConfig;
    public ScenarioConfig CurrentConfig { get => currentConfig;
        set
        {
            currentConfig?.scenarioBtn?.SetSelect(false);
            if (currentConfig == value)
            {
                currentConfig = null;
            }
            else
            {
                currentConfig = value;
            }
            currentConfig?.scenarioBtn?.SetSelect(true);
            startBtn.interactable = currentConfig != null;
        }
    }
    
    bool defaultActiveInitialize;
    List<DefaultActive> defaultActives = new List<DefaultActive>();

    private void Start()
    {
        InitializeScenarioButtons();
        syncAudioList.Add(GetComponent<SyncAudioController>());
    }
    private void InitializeScenarioButtons()
    {
        for (int i = 0; i < scenarioConfigs.Length; i++)
        {
            ScenarioConfig config = scenarioConfigs[i];
            if (config.scenarioBtn == null) continue;

            config.scenarioBtn.button.onClick.AddListener(() =>
            {
                if (!IsOwner) return;
                CurrentConfig = config;
            });
        }

        startBtn.onClick.AddListener(() => {
            if (currentConfig.scenarioPrefab == null) return;
            boardUI.SetActive(false);
            lobbyRoom.SetActive(false);
            int index = scenarioConfigs.ToList().IndexOf(currentConfig);
            SpawnScenarioServerRpc(index);
        });

        disconnectBtn.onClick.AddListener(() =>
        {
            if (VRNetworkController.Instance == null) return;
            VRNetworkController.Instance.Disconnect();
        });
    }

    [ServerRpc]
    void SpawnScenarioServerRpc(int index, ServerRpcParams rpcParams = default)
    {
        if (index < 0 || index >= scenarioConfigs.Length) return;

        var prefab = scenarioConfigs[index].scenarioPrefab;
        if (!prefab) return;

        var go = Instantiate(prefab, transform);

        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(OwnerClientId, true);

        boardUI.SetActive(false);
        lobbyRoom.SetActive(false);

        CCTVController cctv = CCTVController.Instance;
        if ((IsServer || IsHost) && cctv != null)
        {
            CCTVCategory category = CCTVCategory.Scenario1;
            switch (index)
            {
                case 1:
                    category = CCTVCategory.Scenario2;
                    break;
                case 2:
                    category = CCTVCategory.Scenario3;
                    break;
                case 3:
                    category = CCTVCategory.Scenario4;
                    break;
                case 4:
                    category = CCTVCategory.Scenario5;
                    break;
            }

            cctv.SetViewerCategory(OwnerClientId, category);
        }
    }
    Player player;
    PlayerData playerData;
    public override void OnNetworkSpawn()
    {
        headMock.layer = 0;
        player = Player.Instance;
        SetAllCamerasEnabled(false);
        string log = $"[VRManager] Spawn | IsOwner={IsOwner} | ClientId={OwnerClientId}";
        playerData = GetComponent<PlayerData>();

        if (VRNetworkController.Instance != null)//VR
        {
            log += $"| IsInspector: {VRNetworkController.Instance.inspector}";

            if (VRNetworkController.Instance.inspector)
            {
                DisableObjects();
                //player?.SetGravity(false);
                if (IsOwner)
                {
                    boardUI.SetActive(false);
                    lobbyRoom.SetActive(true);
                    environment.SetActive(true);

                    //bool openEnvironment = true;
                    //Scenario[] scenarios = FindObjectsByType<Scenario>(
                    //    FindObjectsInactive.Include,
                    //    FindObjectsSortMode.None
                    //    );
                    //foreach (Scenario scenario in scenarios)
                    //{
                    //    if (scenario.OwnerClientId == TrainingPlayerList.Instance.selectedClientId)
                    //    {
                    //        openEnvironment = false;
                    //    }
                    //}

                    //environment.SetActive(openEnvironment);

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
            }
            else if (!IsOwner)
            {
                DisableObjects();
            }
        }
        else if ((IsServer || IsHost) &&
            CCTVController.Instance != null) // PC side: register camera to CCTV
        {
            headMock.layer = LayerMask.NameToLayer("Mirror");
            CCTVController.Instance.RegisterVRCamera(OwnerClientId, allCamera);
            SetAllCamerasEnabled(true);
            playerData.OnInspector += () => {
                CCTVController.Instance.UnregisterViewer(OwnerClientId);
                SetAllCamerasEnabled(false);
            };
        }

        print(log);
    }

    public void InspectorSetup()
    {
        ulong id = VRNetworkController.Instance.playerId;
        if (id != ulong.MaxValue && id != OwnerClientId)
        {
            return;
        }
        else
        {
            VRNetworkController.Instance.playerId = OwnerClientId;
        }
        ResetObjects();
        playerData.OnDespawn += () =>
        {
            print($"playerData {playerData.OwnerClientId} | VRNetwork {OwnerClientId}");
            VRNetworkController.Instance.Disconnect();
        };

        foreach (var canvas in GetComponentsInChildren<Canvas>())
        {
            VRNetworkController.SetCanvasBlocked(canvas, true);
        }
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log($"[VRManager] Despawn | Owner={OwnerClientId}");

        if (IsServer && CCTVController.Instance != null)
        {
            CCTVController.Instance.UnregisterViewer(OwnerClientId);
        }

        if (IsOwner)
        {
            ShutdownXR();
        }

        player?.SetGravity(true);
        player?.SetMove(true);
        player?.SetJump(true);
        player?.SetTeleportation(true);
        player?.SetTurn(true);
    }

    public void AppendAndSyncCameras(List<Camera> cameras)
    {
        List<Camera> cameraTemp = new List<Camera>();
        foreach (var camera in allCamera) cameraTemp.Add(camera);
        if (cameras != null)
        {
            foreach (var camera in cameras) cameraTemp.Add(camera);
        }

        if (IsServer && CCTVController.Instance != null)
        {
            CCTVController.Instance.UpdateViewer(OwnerClientId, cameraTemp);
        }
    }

    public void SetMute(bool value)
    {
        isMute = value;
        RemoveNullSyncAudio();
        foreach (var item in syncAudioList)
        {
            item?.SetMute(value);
        }
    }

    public void RemoveNullSyncAudio()
    {
        syncAudioList.RemoveAll(item => item == null);
    }

    void SetAllCamerasEnabled(bool value)
    {
        foreach (var camera in allCamera)
        {
            camera.enabled = value;
        }
    }

    public void OpenBoardUI(ulong id)
    {
        if (IsOwner)
        {
            boardUI.SetActive(true);
            lobbyRoom.SetActive(true);
            CurrentConfig = null;
            Player.Instance?.Teleport(Vector3.zero, Vector3.zero);
            return;
        }

        VRNetworkController vRNetworkController = VRNetworkController.Instance;
        TrainingPlayerList trainingPlayerList = TrainingPlayerList.Instance;
        if (!vRNetworkController || !trainingPlayerList) return;
        if (vRNetworkController.inspector &&
            trainingPlayerList.selectedClientId == id
            && trainingPlayerList.isSelected)
        {
            Player.Instance?.Teleport(Vector3.zero, Vector3.zero);
        }
    }

    public void OpenMock() {
        headMock.SetActive(true);
        playerMock.SetActive(true);
    }

    #region XR Control

    void ResetObjects()
    {
        foreach (var item in defaultActives)
        {
            item.go.SetActive(item.value);
        }
    }

    private void DisableObjects()
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

    private void ShutdownXR()
    {
        var ui = FindFirstObjectByType<XRUIInputModule>();
        if (ui) ui.enabled = false;

        var manager = FindFirstObjectByType<XRInteractionManager>();
        if (manager) manager.enabled = false;

        DisableObjects();
    }

    #endregion
}

[System.Serializable]
public class ScenarioConfig
{
    public ScenarioButton scenarioBtn;
    public GameObject scenarioPrefab;
}

[System.Serializable]
public class DefaultActive
{
    public bool value;
    public GameObject go;
}
