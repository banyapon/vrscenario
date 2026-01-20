using UnityEngine;

public class NetworkOwnerComponentDisabler : MonoBehaviour
{
    public Component[] components;
    NetworkOwnershipContext ownership;
    void Start()
    {
        ownership = GetComponentInParent<NetworkOwnershipContext>();
        Apply();
    }
    public void Apply()
    {
        bool isOwner = ownership != null && ownership.IsOwnerClient;

        foreach (var comp in components)
        {
            if (comp == null) continue;

            if (comp is Rigidbody rb)
            {
                rb.isKinematic = !isOwner;
                rb.detectCollisions = isOwner;
            }
            else if (comp is Behaviour behaviour)
            {
                behaviour.enabled = isOwner;
            }
        }
    }
}
