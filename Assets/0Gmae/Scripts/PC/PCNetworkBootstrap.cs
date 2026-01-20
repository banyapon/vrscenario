using System;
using Unity.Netcode;
using UnityEngine;

public class PCNetworkBootstrap : MonoBehaviour
{
    public static PCNetworkBootstrap Instance;

    public GameObject vrPlayerPrefab;
    public float betweenDistance = 10f;
    private int spawnIndex = 0;

    NetworkManager nm;
    public Action<ulong> onClientConnected;
    public Action<ulong> onClientDisconnected;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nm = NetworkManager.Singleton;

        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;
        nm.OnTransportFailure += () =>
        {
            Debug.LogError("[PC] Transport failure");
        };

        Debug.Log("[PC] Starting Host...");
        nm.StartHost();
    }


    void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
            return;

        Debug.Log($"[Server] Remote VR joined: {clientId}");
        float posX = spawnIndex * betweenDistance;
        spawnIndex++;

        GameObject vr = Instantiate(vrPlayerPrefab, new Vector3(posX, 0, 0), Quaternion.identity);

        NetworkObject no = vr.GetComponent<NetworkObject>();
        no.SpawnAsPlayerObject(clientId, true);

        SyncTransformController  sync = vr.GetComponent<SyncTransformController >();
        sync.pcClientId = clientId;
    }


    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("[PC] VR left: " + clientId);
        onClientDisconnected?.Invoke(clientId);
    }
}
