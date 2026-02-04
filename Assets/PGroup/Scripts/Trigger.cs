using UnityEngine;

namespace PGroup
{
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private string triggerTag = "";
        [SerializeField] private GameObject triggerTarget;

        private TriggerController triggerController;

        private void Start()
        {
            triggerController = GetComponentInParent<TriggerController>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (triggerTarget != null)
            {
                if (other.gameObject == triggerTarget)
                {
                    triggerController.GetTrigger(gameObject);
                }
            }
            else
            {
                if (other.CompareTag(triggerTag))
                {
                    triggerController.GetTrigger(gameObject);
                }
            }
        }
    }
}
