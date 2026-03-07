using UnityEngine;

namespace PGroup
{
    public class ActiveTriggerPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject activeObject;
        private Transform player;

        private void OnEnable()
        {
            player = Camera.main.transform;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.transform == player)
            {
                activeObject.SetActive(true);
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
