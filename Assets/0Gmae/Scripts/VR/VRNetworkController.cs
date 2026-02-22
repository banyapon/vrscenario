using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using Unity.Services.Core.Environments;

public class VRNetworkController : MonoBehaviour
{
    public static VRNetworkController Instance;

    public event Action onClientConnected;
    public event Action onClientDisconnected;

    public string joinCode;
    public GameObject vrPlayerPrefab;

    public GameObject[] disconnectedObjects;
    public GameObject[] connectedObjects;

    public TMP_Text statusText;
    public Button clientButton;
    public Button disconnectButton;
    public TMP_InputField inputField;

    NetworkManager nm;
    UnityTransport transport;
    ISession currentSession;

    bool isConnected;

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

        var options = new InitializationOptions()
            .SetEnvironmentName("production");

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Start()
    {
        if (clientButton) clientButton.onClick.AddListener(OnClickJoin);
        if (disconnectButton) disconnectButton.onClick.AddListener(Disconnect);
        if (inputField) inputField.onValueChanged.AddListener((value) =>
        {
            joinCode = value;
        });

        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        ShowDisconnectedUI("Welcome");
    }

    void OnDestroy()
    {
        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientDisconnectCallback -= OnClientDisconnected;

        if (nm != null && nm.IsListening)
            nm.Shutdown();
    }

    async void OnApplicationPause(bool pause)
    {
        if (pause)
            await ForceDisconnect();
    }

    async Task ForceDisconnect()
    {
        if (currentSession != null)
        {
            await currentSession.LeaveAsync();
            currentSession = null;
        }

        if (nm.IsListening)
            nm.Shutdown();

        isConnected = false;
        joinCode = null;

        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }

    async void OnClickJoin()
    {
        await StartRelayClient();
    }

    public async Task StartRelayClient()
    {
        try
        {
            if (string.IsNullOrEmpty(joinCode))
            {
                SetStatus("Join code is empty");
                return;
            }

            joinCode = joinCode.Trim().ToUpper();

            if (nm.IsListening)
                nm.Shutdown();

            SetStatus("Joining...");

            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);

            nm.StartClient();
        }
        catch (Exception e)
        {
            SetStatus("Join Failed");
            Debug.LogError(e);
        }
    }

    public async void Disconnect()
    {
        await ForceDisconnect();
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId)
            return;

        isConnected = true;

        ShowConnectedUI("Connected");
        onClientConnected?.Invoke();
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId)
            return;

        isConnected = false;

        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }

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
}