using UnityEngine;

public class DisableTargetOnEnter : MonoBehaviour
{
    public GameObject target;
    TriggerChecker triggerChecker;

    private void Awake()
    {
        triggerChecker = GetComponent<TriggerChecker>();
    }

    private void OnEnable()
    {
        if (triggerChecker != null)
            triggerChecker.OnEnter += DisableTarget;
    }

    private void OnDisable()
    {
        if (triggerChecker != null)
            triggerChecker.OnEnter -= DisableTarget;
    }

    private void DisableTarget()
    {
        if (target != null)
            target.SetActive(false);
    }
}
