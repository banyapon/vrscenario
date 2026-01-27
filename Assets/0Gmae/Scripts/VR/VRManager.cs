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
    [Header("Camera")]
    [SerializeField] private List<Camera> allCamera = new List<Camera>();

    [Header("Scenario")]
    public GameObject boardUI;
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

    private void Start()
    {
        InitializeScenarioButtons();
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
            if (currentConfig == null) return;
            boardUI.SetActive(false);
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
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[VRManager] Spawn | IsOwner={IsOwner}");

        SetAllCamerasEnabled(false);
        OpenBoardUI();

        if (IsOwner) return;

        // PC side: register camera to CCTV
        if ((IsServer || IsHost) && CCTVController.Instance != null)
        {
            CCTVController.Instance.RegisterVRCamera(OwnerClientId, allCamera);
            SetAllCamerasEnabled(true);
        }
        else
        {
            DisableObjects();
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
    }

    public void AppendAndSyncCameras(List<Camera> cameras)
    {
        RemoveNullCameras();
        foreach (var camera in cameras)
        {
            allCamera.Add(camera);
        }

        if (IsServer && CCTVController.Instance != null)
        {
            CCTVController.Instance.UpdateViewer(OwnerClientId, allCamera);
        }
    }

    public void RemoveNullCameras()
    {
        allCamera.RemoveAll(item => item == null);
    }

    void SetAllCamerasEnabled(bool value)
    {
        foreach (var camera in allCamera)
        {
            camera.enabled = value;
        }
    }

    public void OpenBoardUI()
    {
        boardUI.SetActive(true);
        CurrentConfig = null;
    }

    #region XR Control

    private void DisableObjects()
    {
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
