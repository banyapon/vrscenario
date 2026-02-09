using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
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

    public Action<ulong> onClientConnected;
    public Action<ulong> onClientDisconnected;

    #region INIT

    void Awake()
    {
        Instance = this;

        nm = NetworkManager.Singleton;
        transport = GetComponent<UnityTransport>();
    }

    void Start()
    {
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        nm.OnTransportFailure += () =>
        {
            Debug.LogError("[PC] Transport failure");
        };

        StartHost();
    }

    #endregion

    #region RELAY HOST

    public async void StartHost()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        var code = await CreateSession();
        header.text = $"PC Host";

        if (!string.IsNullOrEmpty(code))
        {
            header.text += $": {code}";
        }
        else
        {
            header.text += $": failed";
            return;
        }

        nm.StartHost();
    }

    public async Task<string> CreateSession()
    {
        try
        {
            var options = new SessionOptions
            {
                Name = "SUT Training Session",
                MaxPlayers = maxPlayer,
                IsPrivate = false,
                IsLocked = false
            };
            options.WithRelayNetwork();

            var session = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log($"Room created successfully! Max players: {session.MaxPlayers}");
            Debug.Log($"Join Code: {session.Code}");

            return session.Code;
        }
        catch (SessionException e)
        {
            Debug.LogError($"Failed to create room: {e.Message}");
            return null;
        }
    }

    #endregion

    #region CLIENT EVENTS

    void OnClientConnected(ulong clientId)
    {
        if (clientId == nm.LocalClientId)
            return;

        Debug.Log($"[PC] VR joined: {clientId}");

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
        Debug.Log("[PC] VR left: " + clientId);
        onClientDisconnected?.Invoke(clientId);
    }

    #endregion
}
