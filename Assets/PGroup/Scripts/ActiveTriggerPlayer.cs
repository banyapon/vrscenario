using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace PGroup
{
    public class ActiveTriggerPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject activeObject;
        [SerializeField] private Transform topPos;
        [SerializeField] private Scenario scenario;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            activeObject.SetActive(true);
            //scenario.SentTeleportToOther(topPos.position);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                activeObject.SetActive(false);
            }
        }
    }
}
