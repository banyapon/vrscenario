using PGroup;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class Thermalscan : MonoBehaviour
    {
        public Action<GameObject, GameObject> OnEnter;
        public Action<GameObject, GameObject> OnExit;

        private Rigidbody rb;
        private XRGrabInteractable grab;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            OnEnter?.Invoke(gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            OnExit?.Invoke(gameObject, other.gameObject);
        }
        void OnGrab(SelectEnterEventArgs args)
        {

        }

        void OnRelease(SelectExitEventArgs args)
        {
            rb.isKinematic = true;
        }
    }
}
