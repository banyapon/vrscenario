using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace PGroup
{
    public class GrabableController : MonoBehaviour
    {
        [SerializeField] private string animationName;

        private XRGrabInteractable grab;
        private Rigidbody rb;
        private Animation anim;

        private void Awake()
        {
            grab = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animation>();

            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
        private void Start()
        {
            rb.isKinematic = true;
        }

        private void OnGrab(SelectEnterEventArgs arg)
        {
            rb.isKinematic = false;
            if (anim != null) PlayAnimation(anim, animationName, false);
        }
        private void OnRelease(SelectExitEventArgs arg)
        {
            rb.isKinematic = true;
            if (anim != null) PlayAnimation(anim, animationName, true);
        }
        private void PlayAnimation(Animation animation, string clip, bool reversed)
        {
            if (!reversed)
            {
                animation[clip].speed = 1f;
                animation[clip].time = 0;
                animation.PlayQueued(clip);
            }
            else
            {
                animation[clip].speed = -1f;
                animation[clip].time = animation[clip].length;
                animation.Play(clip);
            }
        }
    }
}
