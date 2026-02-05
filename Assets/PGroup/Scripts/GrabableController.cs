using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabableController : MonoBehaviour
{
    private XRGrabInteractable grab;
    private Rigidbody rb;
    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs arg)
    {
        rb.isKinematic = false;
    }
    private void OnRelease(SelectExitEventArgs arg)
    {
        rb.isKinematic = true;
    }
}
