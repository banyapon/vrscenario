using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class PCNetworkBootstrap : MonoBehaviour
{
    public static PCNetworkBootstrap Instance;

    public GameObject vrPlayerPrefab;
    public float betweenDistance = 10f;

    int spawnIndex = 0;

    NetworkManager nm;
    UnityTransport transport;

    public Action<ulong> onClientConnected;
    public Action<ulong> onClientDisconnected;

    #region INIT

    async void Awake()
    {
        Instance = this;

        nm = NetworkManager.Singleton;
        transport = GetComponent<UnityTransport>();

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async void Start()
    {
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        nm.OnTransportFailure += () =>
        {
            Debug.LogError("[PC] Transport failure");
        };

        await StartRelayHost();
    }

    #endregion

    #region RELAY HOST

    async Task StartRelayHost()
    {
        Debug.Log("[PC] Creating Relay Host...");

        Allocation allocation =
            await RelayService.Instance.CreateAllocationAsync(8);

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

        Debug.Log($"[PC] Relay Join Code: {joinCode}");
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

        GameObject vr =
            Instantiate(vrPlayerPrefab, new Vector3(posX, 0, 0), Quaternion.identity);

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
