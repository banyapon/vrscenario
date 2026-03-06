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
using Boy;
using System.Collections.Generic;

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
    Dictionary<ulong, ConnectionPayload> clientPayloads = new();

    void Awake()
    {
        Instance = this;
        nm = NetworkManager.Singleton;
        transport = GetComponent<UnityTransport>();

        nm.ConnectionApprovalCallback += ApprovalCheck;
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
        APIManager.Instance.SaveJoinCode<string>(code);
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

        if (!clientPayloads.TryGetValue(clientId, out ConnectionPayload payload))
        {
            Debug.LogWarning($"Payload not ready for client {clientId}");
            return;
        }

        if (payload.isInspector)
        {
            Debug.Log($"Client {clientId} is Inspector");
            return;
        }

        float posX = spawnIndex * betweenDistance;
        spawnIndex++;

        GameObject vr = Instantiate(vrPlayerPrefab, new Vector3(posX, 0, 0), Quaternion.identity);

        NetworkObject no = vr.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(clientId, true);

        var sync = vr.GetComponent<SyncTransformController>();
        sync.pcClientId = clientId;

        var playerData = vr.GetComponent<PlayerData>();
        playerData.IsInspector.Value = payload.isInspector;

        onClientConnected?.Invoke(clientId);
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId == nm.LocalClientId)
            return;

        onClientDisconnected?.Invoke(clientId);
    }
    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
                   NetworkManager.ConnectionApprovalResponse response)
    {
        ConnectionPayload payload = new ConnectionPayload();

        if (request.Payload != null && request.Payload.Length > 0)
        {
            string json = System.Text.Encoding.UTF8.GetString(request.Payload);
            payload = JsonUtility.FromJson<ConnectionPayload>(json);
        }

        clientPayloads[request.ClientNetworkId] = payload;

        Debug.Log($"Client {request.ClientNetworkId} | Inspector: {payload.isInspector} | Name: {payload.userName}");

        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    public string GetUserName(ulong clientId)
    {
        if (!clientPayloads.TryGetValue(clientId, out ConnectionPayload payload))
        {
            return "";
        }

        return payload.userName;
    }
}

[Serializable]
public struct ConnectionPayload
{
    public bool isInspector;
    public string userName;
}