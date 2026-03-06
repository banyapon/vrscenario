using Unity.Netcode;
using UnityEngine;

namespace PGroup
{
    public class MultiplayerController : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnected;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) return;

            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnected;
        }

        private void OnPlayerConnected(ulong clientId)
        {
            Debug.Log("Player Connected : " + clientId);

            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            Debug.Log("Players in room : " + playerCount);
        }

        private void OnPlayerDisconnected(ulong clientId)
        {
            Debug.Log("Player Disconnected : " + clientId);

            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            Debug.Log("Players in room : " + playerCount);
        }
    }
}
