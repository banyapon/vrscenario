using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform followPos;
    private void Update()
    {
        transform.position = followPos.position;
        transform.rotation = followPos.rotation;
    }
}
