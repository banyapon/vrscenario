using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class Hook : MonoBehaviour
    {
        public bool isHit;
        public Transform player;
        public Vector3 isFollowPlayer;
        public Action OnEnter;
        public Action OnExit;
        public GameObject hitObject;
        public float smoothTime = 0.2f;

        private Animation anim; 
        private Vector3 velocity;
        private Vector3 startOffset;
        private bool onGrab;

        void Awake()
        {
            anim = GetComponent<Animation>();
            player = Camera.main.transform.parent.parent;
            startOffset = transform.position - player.position;

            var grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
        private void LateUpdate()
        {
            if (isFollowPlayer == Vector3.zero || onGrab) return;

            //GetComponent<Rigidbody>().isKinematic = true;
            Vector3 current = transform.position;
            Vector3 target = player.position + startOffset;

            Vector3 followTarget = new Vector3(
                isFollowPlayer.x == 1 ? target.x : current.x,
                isFollowPlayer.y == 1 ? target.y : current.y,
                isFollowPlayer.z == 1 ? target.z : current.z
                );

            transform.position = Vector3.SmoothDamp(
                current,
                followTarget,
                ref velocity,
                smoothTime
            );
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
            onGrab = true;
            GetComponent<Rigidbody>().isKinematic = false;
            PlayAnimation(anim, "HookGrab", false);
        }

        void OnRelease(SelectExitEventArgs args)
        {
            onGrab = false;
            startOffset = transform.position - player.position;
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
