using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class Hook : MonoBehaviour
    {
        public bool isHit;
        public Action OnEnter;
        public Action OnExit;
        public GameObject hitObject;

        private Animation anim;

        void Awake()
        {
            anim = GetComponent<Animation>();

            var grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag("Ladder"))
            {
                isHit = true;
                hitObject = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag("Ladder"))
            {
                isHit = false;
                hitObject = null;
            }
        }

        void OnGrab(SelectEnterEventArgs args)
        {
            PlayAnimation(anim, "HookGrab", false);
        }

        void OnRelease(SelectExitEventArgs args)
        {
            PlayAnimation(anim, "HookGrab", true);
            if (isHit)
            {
                isHit = false;
                OnEnter?.Invoke();
            }
            else
            {
                OnExit?.Invoke();
            }
            GetComponent<Rigidbody>().isKinematic = true;
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
