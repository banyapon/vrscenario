using System;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VRNetworkController : MonoBehaviour
{
    public static VRNetworkController Instance;

    // ===============================
    // Events (public)
    // ===============================
    public event Action onClientConnected;
    public event Action onClientDisconnected;

    public GameObject vrPlayerPrefab;

    [Header("UI Groups")]
    public GameObject[] disconnectedObjects;
    public GameObject[] connectedObjects;

    [Header("UI")]
    public TMP_Text statusText;
    public Button clientButton;
    public Button hostButton;
    public Button disconnectButton;

    NetworkManager nm;
    bool isConnected;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nm = NetworkManager.Singleton;

        // Buttons
        if (clientButton) clientButton.onClick.AddListener(StartClient);
        if (hostButton) hostButton.onClick.AddListener(StartHost);
        if (disconnectButton) disconnectButton.onClick.AddListener(Disconnect);

        // NGO callbacks
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        ShowDisconnectedUI("Welcome");
    }

    void OnDestroy()
    {
        if (nm == null) return;

        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    // ===============================
    // Network Actions
    // ===============================

    public void StartClient()
    {
        if (nm.IsClient || nm.IsHost) return;

        SetStatus("Connecting...");
        nm.StartClient();
    }

    public void StartHost()
    {
        if (nm.IsHost) return;

        SetStatus("Starting Host...");
        nm.StartHost();
    }

    public void Disconnect()
    {
        if (!nm.IsClient && !nm.IsHost) return;

        nm.Shutdown();
        VRAvatarNetwork.Local = null;

        isConnected = false;
        onClientDisconnected?.Invoke();

        ShowDisconnectedUI("Disconnected");
    }

    // ===============================
    // NGO Callbacks
    // ===============================

    void OnClientConnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId) return;

        Debug.Log($"[VR] Connected id={clientId}");

        isConnected = true;

        ShowConnectedUI("Connected");
        onClientConnected?.Invoke();

        if (!nm.IsHost) return;
        GameObject vr = Instantiate(vrPlayerPrefab);
        NetworkObject no = vr.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(clientId, true);
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId) return;

        Debug.Log($"[VR] Disconnected id={clientId}");

        isConnected = false;
        VRAvatarNetwork.Local = null;

        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }

    // ===============================
    // UI State Handling
    // ===============================

    void ShowConnectedUI(string text)
    {
        SetGroupActive(disconnectedObjects, false);
        SetGroupActive(connectedObjects, true);

        SetStatus(text);

        //if (disconnectButton) disconnectButton.gameObject.SetActive(true);
    }

    void ShowDisconnectedUI(string text)
    {
        SetGroupActive(disconnectedObjects, true);
        SetGroupActive(connectedObjects, false);

        SetStatus(text);

        //if (disconnectButton) disconnectButton.gameObject.SetActive(false);
    }

    void SetGroupActive(GameObject[] objects, bool active)
    {
        if (objects == null) return;

        foreach (var go in objects)
        {
            if (go) go.SetActive(active);
        }
    }

    void SetStatus(string text)
    {
        if (statusText) statusText.text = text;
    }
}
