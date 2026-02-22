using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using Unity.Services.Core.Environments;
using TMPro;

public class PCNetworkBootstrap : MonoBehaviour
{
    public static PCNetworkBootstrap Instance;

    public GameObject vrPlayerPrefab;
    public TMP_Text header;
    public float betweenDistance = 10f;
    public int maxPlayer = 16;

    int spawnIndex = 0;

    NetworkManager nm;
    UnityTransport transport;

    ISession currentSession;

    public Action<ulong> onClientConnected;
    public Action<ulong> onClientDisconnected;

    void Awake()
    {
        Instance = this;
        nm = NetworkManager.Singleton;
        transport = GetComponent<UnityTransport>();
    }

    async void Start()
    {
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        await RelayAuthen();
        await StartHost();
    }

    async Task StartHost()
    {
        var code = await CreateSession();

        if (string.IsNullOrEmpty(code))
        {
            header.text = "PC Host: failed";
            return;
        }

        header.text = $"PC Host: {code}";
        print(code);
        nm.StartHost();
    }

    async Task<string> CreateSession()
    {
        var options = new SessionOptions
        {
            Name = "SUT Training Session",
            MaxPlayers = maxPlayer,
            IsPrivate = false,
            IsLocked = false
        };

        options.WithRelayNetwork();

        currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        return currentSession.Code;
    }

    async Task RelayAuthen()
    {
        var options = new InitializationOptions()
            .SetEnvironmentName("production");

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId == nm.LocalClientId)
            return;

        float posX = spawnIndex * betweenDistance;
        spawnIndex++;

        GameObject vr = Instantiate(vrPlayerPrefab, new Vector3(posX, 0, 0), Quaternion.identity);

        NetworkObject no = vr.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(clientId, true);

        var sync = vr.GetComponent<SyncTransformController>();
        sync.pcClientId = clientId;

        onClientConnected?.Invoke(clientId);
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId == nm.LocalClientId)
            return;

        onClientDisconnected?.Invoke(clientId);
    }
}