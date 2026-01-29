using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

namespace Boy
{
    public class Hook : MonoBehaviour
    {
        public bool isLeftSide = true;

        [Header("Transform Setting")]
        public float holdDuration = 2;
        public Transform target;

        [Header("Lock Setting")]
        public float lockDuration = 0.25f;
        public Transform lockModel;
        public Transform lockTarget;

        Tween resetTween;
        Tween lockTween;
        Rigidbody rb;
        VRInput vRInput;

        HashSet<Collider> handColliders = new HashSet<Collider>();
        HashSet<Collider> ladderColliders = new HashSet<Collider>();

        bool IsInsideHand => handColliders.Count > 0;
        bool IsInsideLadder => ladderColliders.Count > 0;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            vRInput = Player.Instance.vRInput;
        }

        private void Update()
        {
            if (vRInput == null) return;

            if ((vRInput.primaryLeft && isLeftSide) ||
                (vRInput.primaryRight && !isLeftSide))
            {
                ResetTransform();
            }
        }

        public void ResetTransform()
        {
            if (!rb) rb = GetComponent<Rigidbody>();

            SetGravity(false);
            resetTween?.Kill();
            resetTween = DOVirtual.DelayedCall(holdDuration, () =>
            {
                SetGravity(true);
            });

            if (target)
            {
                transform.localPosition = target.localPosition;
                transform.localEulerAngles = target.localEulerAngles;
            }
            else
            {
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                resetTween?.Kill();
                handColliders.Add(other);

                SetLockRotate(true);
                UpdateGravity();
            }
            else if (other.CompareTag("Ladder"))
            {
                resetTween?.Kill();
                ladderColliders.Add(other);
                UpdateGravity();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                handColliders.Remove(other);

                if (!IsInsideHand) SetLockRotate(false);

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
            SetGravity(shouldEnableGravity);
        }

        void SetGravity(bool value)
        {
            rb.useGravity = value;
            rb.isKinematic = !value;
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
