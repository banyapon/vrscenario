using UnityEngine;

namespace PGroup
{
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private string triggerTag = "";
        [SerializeField] private GameObject triggerTarget;
        [SerializeField] private GameObject correctTrigger;
        [SerializeField] private GameObject wrongTrigger;

        private TriggerController triggerController;

        private void Start()
        {
            triggerController = GetComponentInParent<TriggerController>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (triggerTarget != null && !string.IsNullOrEmpty(triggerTag))
            {
                if (other.CompareTag(triggerTag))
                {
                    if (other.gameObject == triggerTarget)
                    {
                        other.gameObject.SetActive(false);
                        correctTrigger.SetActive(true);
                        triggerController.GetTrigger(gameObject);
                    }
                    else
                    {
                        wrongTrigger.SetActive(true);
                    }
                }
            }
            else if (triggerTarget != null)
            {
                if (other.gameObject == triggerTarget)
                {
                    other.gameObject.SetActive(false);
                    correctTrigger.SetActive(true);
                    triggerController.GetTrigger(gameObject);
                }
            }
            else
            {
                if (other.CompareTag(triggerTag))
                {
                    other.gameObject.SetActive(false);
                    correctTrigger.SetActive(true);
                    triggerController.GetTrigger(gameObject);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == triggerTarget)
            {
                if (wrongTrigger.activeSelf)
                {
                    wrongTrigger.SetActive(false);
                }
            }
        }
    }
}
