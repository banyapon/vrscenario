using UnityEngine;

namespace PGroup
{
    public class TriggerTeleport : MonoBehaviour
    {
        [SerializeField] private Transform toPos;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>())
            {
                Player.Instance.Teleport(toPos);
            }
        }
    }
}
