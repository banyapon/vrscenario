using UnityEngine;

public class AwakeAction : MonoBehaviour
{
    [SerializeField] private bool active;
    private void Start()
    {
        gameObject.SetActive(active);
    }
}
