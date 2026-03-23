using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace PGroup
{
    public class ActiveTriggerPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject activeObject;
        [SerializeField] private Transform topPos;

        private bool isPlayer;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            activeObject.SetActive(true);

            if (IsOwner)
            {
                isPlayer = true;
                TeleportServerRpc();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                activeObject.SetActive(false);
            }
        }

        [ServerRpc]
        private void TeleportServerRpc()
        {
            TeleportClientRpc();
        }

        [ClientRpc]
        private void TeleportClientRpc()
        {
            if (isPlayer) return;
            GetComponent<Collider>().enabled = false;
            Player.Instance.Teleport(topPos, false);
        }
    }
}
