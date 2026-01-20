using System;
using UnityEngine;

public class TriggerChecker : MonoBehaviour
{
    public string triggerTag = "Hand";
    public GameObject triggerTarget;

    public Action OnEnter;
    public Action OnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (IsValidTarget(other)) OnEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;
        if (IsValidTarget(other)) OnExit?.Invoke();
    }

    private bool IsValidTarget(Collider other)
    {
        if (triggerTarget != null)
        {
            return other.gameObject == triggerTarget;
        }

        if (!string.IsNullOrEmpty(triggerTag))
        {
            return other.CompareTag(triggerTag);
        }

        return true;
    }
}
