using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class VRNetworkController : MonoBehaviour
{
    public static VRNetworkController Instance;

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
    UnityTransport transport;

    bool isConnected;

    #region INIT

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        nm = NetworkManager.Singleton;
        transport = GetComponent<UnityTransport>();

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Start()
    {
        if (clientButton) clientButton.onClick.AddListener(() => StartRelayClient("PUT_JOIN_CODE"));
        if (hostButton) hostButton.onClick.AddListener(StartLocalHost);
        if (disconnectButton) disconnectButton.onClick.AddListener(Disconnect);

        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        ShowDisconnectedUI("Welcome");
    }

    void OnDestroy()
    {
        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    #endregion

    #region SAFE STOP

    void StopIfRunning()
    {
        if (nm.IsListening)
            nm.Shutdown();
    }

    #endregion

    #region LOCAL HOST (VR SOLO)

    public void StartLocalHost()
    {
        StopIfRunning();

        transport.SetConnectionData("0.0.0.0", 7777);

        SetStatus("Starting Local Host...");
        nm.StartHost();
    }

    #endregion

    #region RELAY CLIENT (JOIN PC)

    public async void StartRelayClient(string joinCode)
    {
        StopIfRunning();

        SetStatus("Joining Relay...");

        JoinAllocation allocation =
            await RelayService.Instance.JoinAllocationAsync(joinCode);

        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        nm.StartClient();
    }

    #endregion

    #region RELAY HOST (OPTIONAL)

    public async Task<string> StartRelayHost(int maxPlayers = 4)
    {
        StopIfRunning();

        SetStatus("Creating Relay Room...");

        Allocation allocation =
            await RelayService.Instance.CreateAllocationAsync(maxPlayers);

        string joinCode =
            await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.ConnectionData
        );

        nm.StartHost();

        Debug.Log("Join Code: " + joinCode);

        return joinCode;
    }

    #endregion

    #region DISCONNECT

    public void Disconnect()
    {
        StopIfRunning();

        isConnected = false;
        onClientDisconnected?.Invoke();

        ShowDisconnectedUI("Disconnected");
    }

    #endregion

    #region NGO CALLBACKS

    void OnClientConnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId) return;

        isConnected = true;

        ShowConnectedUI("Connected");
        onClientConnected?.Invoke();

        if (!nm.IsHost) return;

        GameObject vr = Instantiate(vrPlayerPrefab);
        vr.GetComponent<NetworkObject>()
          .SpawnAsPlayerObject(clientId, true);
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId) return;

        isConnected = false;

        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }

    #endregion

    #region UI

    void ShowConnectedUI(string text)
    {
        SetGroupActive(disconnectedObjects, false);
        SetGroupActive(connectedObjects, true);
        SetStatus(text);
    }

    void ShowDisconnectedUI(string text)
    {
        SetGroupActive(disconnectedObjects, true);
        SetGroupActive(connectedObjects, false);
        SetStatus(text);
    }

    void SetGroupActive(GameObject[] objs, bool active)
    {
        foreach (var go in objs)
            if (go) go.SetActive(active);
    }

    void SetStatus(string t)
    {
        if (statusText) statusText.text = t;
    }

    #endregion
}
