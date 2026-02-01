using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabChecker : MonoBehaviour
{
    public Action OnGrab;
    public Action OnRelease;

    private void Awake()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab == null) return;

        grab.selectEntered.AddListener((_) => {
            if (enabled == false) return;
            OnGrab?.Invoke();
        });

        grab.selectExited.AddListener((_) => {
            if (enabled == false) return;
            OnRelease?.Invoke();
        });
    }
}
