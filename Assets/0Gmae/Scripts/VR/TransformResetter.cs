using UnityEngine;

public class TransformResetter : MonoBehaviour
{
    Vector3 originalPos;
    Vector3 originalEuler;

    Rigidbody rb;

    private void Awake()
    {
        originalPos = transform.localPosition;
        originalEuler = transform.localEulerAngles;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ResetArea")) ResetTransform();
    }

    public void ResetTransform()
    {
        transform.localPosition = originalPos;
        transform.localEulerAngles = originalEuler;

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
