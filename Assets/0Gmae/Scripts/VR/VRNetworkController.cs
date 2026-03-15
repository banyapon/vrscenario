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
using Boy;

public class VRNetworkController : MonoBehaviour
{
    public static VRNetworkController Instance;

    public event Action onClientConnected;
    public event Action onClientDisconnected;

    //public string joinCode;

    public bool inspector;
    public ulong playerId = ulong.MaxValue;

    public GameObject[] disconnectedObjects;
    public GameObject[] connectedObjects;

    public TMP_Text statusText;
    public Button clientButton;
    public Button hostButton;
    public Button disconnectButton;
    public TMP_InputField inputField;
    public GameObject hostPrefab;
    public GameObject logOutUI;
    public GameObject playerDropdown;
    GameObject spawnedHostObject;

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
        logOutUI.SetActive(false);
        playerDropdown.SetActive(false);

        transport = GetComponent<UnityTransport>();

        var options = new InitializationOptions()
            .SetEnvironmentName("production");

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    void Start()
    {
        if (clientButton) clientButton.onClick.AddListener(() =>
        {
            OnClickJoin(false);
        });
        if (hostButton) hostButton.onClick.AddListener(StartHostLocal);
        if (disconnectButton) disconnectButton.onClick.AddListener(Disconnect);
        if (inputField) inputField.onValueChanged.AddListener((value) =>
        {
            //joinCode = value;
        });

        nm = NetworkManager.Singleton;
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

    void OnApplicationPause(bool pause)
    {
        if (!pause) return;
        _ = HandlePauseAsync();
    }
    void OnClickHost()
    {
        _ = StartHostSession();
    }
    async Task StartHostSession()
    {
        if (nm.IsListening)
            nm.Shutdown();

        SetStatus("Creating session...");

        try
        {
            var options = new SessionOptions
            {
                Name = "SUT Training Session",
                MaxPlayers = 1,
                IsPrivate = false,
                IsLocked = false
            };

            options.WithRelayNetwork();
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            string joinCode = currentSession.Code;
            Debug.Log("Join Code: " + joinCode);

            nm.StartHost();

            SetStatus("Hosting\nCode: " + joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogError("Host Failed: " + ex.Message);
            SetStatus("Failed to create host session");
        }
    }

    async Task HandlePauseAsync()
    {
        if (currentSession != null)
        {
            await currentSession.LeaveAsync();
            currentSession = null;
        }

        if (nm.IsListening) nm.Shutdown();

        DespawnHostObject();
        isConnected = false;
        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }

    bool isJoining;
    public void OnClickJoin(bool isInspector)
    {
        if (isJoining) return;
        isJoining = true;
        print($"OnClickJoin: {isInspector}");
        inspector = isInspector;
        Player.Instance.SetInspector(!inspector);
        APIManager.Instance.GetJoinCode<JoinMultiplayerResponse>(OnJoinCodeReceived);
    }

    async void OnJoinCodeReceived(bool status, string message, JoinMultiplayerResponse response)
    {
        try
        {
            Debug.Log(message);

            if (!status || response == null || response.data == null)
            {
                SetStatus("Failed to get join code");
                isJoining = false;
                return;
            }

            await StartRelayClient(response.data.key_join_multiplayer);
        }
        catch (Exception ex)
        {
            Debug.LogError("Join flow crashed: " + ex.Message);
            SetStatus("Join error");
            isJoining = false;
        }
    }
    public async Task StartRelayClient(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            SetStatus("Join code is empty");
            isJoining = false;
            return;
        }

        joinCode = joinCode.Trim().ToUpper();

        if (nm.IsListening)
            nm.Shutdown();

        SetStatus("Connecting...");

        try
        {
            ConnectionPayload payload = new ConnectionPayload
            {
                isInspector = inspector,
                userName = APIManager.Instance.userEmail
            };

            string json = JsonUtility.ToJson(payload);
            nm.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(json);

            currentSession = await MultiplayerService.Instance
                .JoinSessionByCodeAsync(joinCode);
            nm.StartClient();

            SetStatus("Connected!");
            logOutUI.SetActive(inspector);
            playerDropdown.SetActive(inspector);
        }
        catch (Exception ex)
        {
            Debug.LogError("Join Failed: " + ex.Message);
            SetStatus("Invalid or expired join code");
        }
        finally
        {
            isJoining = false;
        }
    }

    public async void Disconnect()
    {
        if (currentSession != null)
        {
            await currentSession.LeaveAsync();
            currentSession = null;
        }

        if (nm.IsListening)
            nm.Shutdown();

        DespawnHostObject();

        playerId = ulong.MaxValue;
        isConnected = false;
        isJoining = false;
        ShowDisconnectedUI("Disconnected");
        logOutUI.SetActive(false);
        playerDropdown.SetActive(false);
        onClientDisconnected?.Invoke();
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId)
            return;

        isConnected = true;
        ShowConnectedUI("Connected");
        onClientConnected?.Invoke();
        if (nm.IsHost) SpawnHostObject(clientId);
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId != nm.LocalClientId)
            return;

        isConnected = false;
        ShowDisconnectedUI("Disconnected");
        onClientDisconnected?.Invoke();
    }
    void SpawnHostObject(ulong clientId)
    {
        if (hostPrefab == null)
            return;

        if (spawnedHostObject != null)
            return;

        spawnedHostObject = Instantiate(hostPrefab);
        NetworkObject no = spawnedHostObject.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(clientId, true);
    }
    void DespawnHostObject()
    {
        if (spawnedHostObject != null)
        {
            Destroy(spawnedHostObject);
            spawnedHostObject = null;
        }
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
    public void StartHostLocal()
    {
        if (nm.IsListening)
            nm.Shutdown();

        ConfigureTransportAsHost();

        bool success = nm.StartHost();

        if (success)
        {
            SetStatus("Hosting (No Relay)");
        }
        else
        {
            SetStatus("Failed to start host");
        }
    }
    void ConfigureTransportAsHost()
    {
        transport.SetConnectionData(
            "0.0.0.0",
            7777
        );
    }

    public static void SetCanvasBlocked(Canvas canvas, bool blocked)
    {
        var cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.gameObject.AddComponent<CanvasGroup>();

        cg.interactable = !blocked;
        cg.blocksRaycasts = !blocked;
    }
}