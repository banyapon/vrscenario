using UnityEngine;

namespace PGroup
{
    public class ActiveTriggerPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject activeObject;
        [SerializeField] private Transform topPos;
        private Transform player;

        private void OnEnable()
        {
            player = Camera.main.transform;
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
            if (other.transform == player)
            {
                activeObject.SetActive(true);
            }
            else if (other.CompareTag("Player"))
            {
                activeObject.SetActive(true);
                player.GetComponent<Player>().TeleportNonOwner(topPos, false);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.transform == player)
            {
                activeObject.SetActive(false);
            }
        }
    }
}
