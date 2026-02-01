using UnityEngine;

public class ForceIsKinematic : MonoBehaviour
{
    public float delaySet = 1;
    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) return;
        Invoke(nameof(Force), delaySet);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Force));
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(Force));
    }

    void Force()
    {
        rb.isKinematic = true;
    }
}
