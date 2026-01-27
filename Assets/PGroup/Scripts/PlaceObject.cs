using UnityEngine;
using UnityEngine.Events;

namespace PGroup
{
    public class PlaceObject : MonoBehaviour
    {
        [SerializeField] private string processTag;
        [SerializeField] private UnityEvent OnSuccess;
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(processTag))
            {
                transform.GetChild(0).gameObject.SetActive(false);
                OnSuccess?.Invoke();
            }
        }
    }
}
