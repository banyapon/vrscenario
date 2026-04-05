using DG.Tweening;
using UnityEngine;

namespace PGroup
{
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private string triggerTag = "";
        [SerializeField] private GameObject triggerTarget;
        [SerializeField] private GameObject correctTrigger;
        [SerializeField] private GameObject wrongTrigger;
        [SerializeField] private GameObject deactiveObject;
        [SerializeField] private bool deactiveTarget;

        private TriggerController triggerController;
        private Tween delay = null;
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            triggerController = GetComponentInParent<TriggerController>();

            if (triggerTarget != null) triggerTarget.GetComponent<Collider>().enabled = true;
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
                        _collider.enabled = false;
                        if (deactiveObject != null) deactiveObject.SetActive(false);
                        if (deactiveTarget) other.gameObject.SetActive(false);
                        if (correctTrigger != null) correctTrigger.SetActive(true);
                        triggerController.GetTrigger(gameObject);
                    }
                    else
                    {
                        _collider.enabled = true;
                        WrongTrigger(other.gameObject);
                    }
                }
            }
            else if (triggerTarget != null)
            {
                if (other.gameObject == triggerTarget)
                {
                    _collider.enabled = false;
                    if (deactiveObject != null) deactiveObject.SetActive(false);
                    if (deactiveTarget) other.gameObject.SetActive(false);
                    if (correctTrigger != null) correctTrigger.SetActive(true);
                    triggerController.GetTrigger(gameObject);
                }
            }
            else
            {
                if (other.CompareTag(triggerTag))
                {
                    _collider.enabled = false;
                    if (deactiveObject != null) deactiveObject.SetActive(false);
                    if (deactiveTarget) other.gameObject.SetActive(false);
                    if (correctTrigger != null) correctTrigger.SetActive(true);
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

        private void WrongTrigger(GameObject target)
        {
            target.SetActive(false);
            wrongTrigger.SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                wrongTrigger.SetActive(false);
            });
        }
    }
}
