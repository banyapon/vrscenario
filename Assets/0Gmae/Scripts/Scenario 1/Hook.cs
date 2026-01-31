using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Boy
{
    public class Hook : MonoBehaviour
    {
        public bool isLeftSide = true;
        public TriggerChecker checker;

        [Header("Transform Setting")]
        public Vector3 offset;

        [Header("Lock Setting")]
        public float lockDuration = 0.25f;
        public Transform lockModel;
        public Transform lockTarget;

        Tween lockTween;
        Rigidbody rb;
        VRInput vRInput;
        XRGrabInteractable grab;

        HashSet<Collider> handColliders = new HashSet<Collider>();
        HashSet<Collider> ladderColliders = new HashSet<Collider>();

        bool IsInsideHand => handColliders.Count > 0;
        bool IsInsideLadder => ladderColliders.Count > 0;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            vRInput = Player.Instance.vRInput;

            grab = GetComponent<XRGrabInteractable>();
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }

        private void OnEnable()
        {
            ResetTransform();
        }

        bool wasPressed = false;
        private void Update()
        {
            if (vRInput == null)
            {
                vRInput = Player.Instance.vRInput;
                return;
            }

            bool left = vRInput.primaryLeft && isLeftSide;
            bool right = vRInput.primaryRight && !isLeftSide;

            bool isPressed = left || right;

            if (isPressed && !wasPressed) ResetTransform();

            wasPressed = isPressed;
        }

        public void ResetTransform()
        {
            if (!rb) rb = GetComponent<Rigidbody>();

            SetGravity(false);

            transform.SetParent(null);

            Transform camera = Player.Instance.camera.transform;
            transform.localPosition = camera.position + camera.rotation * offset;
            transform.localEulerAngles = camera.eulerAngles;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                handColliders.Add(other);

                //SetLockRotate(true);
                UpdateGravity();
            }
            else if (other.CompareTag("Ladder"))
            {
                ladderColliders.Add(other);
                UpdateGravity();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                handColliders.Remove(other);

                //if (!IsInsideHand) SetLockRotate(false);

                UpdateGravity();
            }
            else if (other.CompareTag("Ladder"))
            {
                ladderColliders.Remove(other);
                UpdateGravity();
            }
        }

        void UpdateGravity()
        {
            bool shouldEnableGravity = !IsInsideHand && !IsInsideLadder;
            //SetGravity(shouldEnableGravity);
        }

        void SetGravity(bool value)
        {
            rb.useGravity = value;
            rb.isKinematic = !value;
        }
        void OnGrab(SelectEnterEventArgs args)
        {
            SetLockRotate(true);
        }
        void OnRelease(SelectExitEventArgs args)
        {
            SetLockRotate(false);
        }

        public void SetLockRotate(bool open)
        {
            lockTween?.Kill();

            Vector3 target = open ? lockTarget.localEulerAngles : Vector3.zero;
            lockTween = lockModel
                .DOLocalRotate(target, lockDuration)
                .SetLink(gameObject);
        }
    }
}
