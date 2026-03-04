using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class Hook : MonoBehaviour
    {
        public bool isHit;
        public Transform follow;
        public Action<GameObject,GameObject> OnEnter;
        public Action OnExit;
        public GameObject hitObject;

        private Animation anim; 
        private bool onGrab;

        void Awake()
        {
            anim = GetComponent<Animation>();

            var grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
        public void SetFollower(Transform follower)
        {
            follow = follower;
        }
        private void LateUpdate()
        {
            if (follow != null)
            {
                transform.position = follow.position;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag("Ladder"))
            {
                isHit = true;
                hitObject = other.gameObject;
            }
            else if (other.GetComponent<PlaceObject>())
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
            onGrab = true;
            GetComponent<Rigidbody>().isKinematic = false;
            PlayAnimation(anim, "HookGrab", false);
        }

        void OnRelease(SelectExitEventArgs args)
        {
            onGrab = false;
            PlayAnimation(anim, "HookGrab", true);
            if (isHit)
            {
                isHit = false;
                OnEnter?.Invoke(gameObject,hitObject);
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
