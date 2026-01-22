using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void Teleport(Transform transform)
    {
        if (transform == null) return;
        Teleport(transform.localPosition, transform.localEulerAngles);
    }

    public void Teleport(Vector3 position, Vector3 rotate)
    {
        transform.position = position;
        transform.eulerAngles = rotate;
    }
}
