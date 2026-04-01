using DG.Tweening;
using System;
using UnityEngine;

namespace Boy
{
    public class Victims : MonoBehaviour
    {
        [SerializeField] GameObject unconsciousCollider;
        [SerializeField] Collider[] colliders;
        [SerializeField] NPC npc;
        [SerializeField] Transform model;
        [SerializeField] GameObject safetyHarness;
        [SerializeField] private AnimationClip unconsciousClip;
        [SerializeField] private AnimationClip pullupClip;

        Tween animationTween;
        Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Unconscious(Action callback = null)
        {
            npc.SetForcePosition(false, 0);
            npc.SetForceRotation(true, 0);
            npc.SetBool("look", false);
            npc.SetBool("pull up", false);
            npc.SetBool("unconscious", true);
            DOTween.Kill(model);
            //model.DOLocalRotate(Vector3.zero, 1);
            PlayAnimation("unconscious", unconsciousClip.length, callback);
            SwitchCollider(false);
        }

        public void Pullup(Action callback = null)
        {
            npc.SetBool("look", false);
            npc.SetBool("unconscious", false);
            npc.SetBool("pull up", true);
            npc.SetForcePosition(true);
            npc.SetForceRotation(true);
            safetyHarness.SetActive(true);
            PlayAnimation("pull up", pullupClip.length, callback);
        }

        public void PlayAnimation(string trigger, float duration = 0, Action callback = null)
        {
            animator.SetBool("reset", false);
            animator.SetBool("unconscious", false);
            animator.SetBool("pull up", false);

            animator.SetBool(trigger, true);

            animationTween?.Kill();
            animationTween = DOVirtual.DelayedCall(duration, () => {
                callback?.Invoke();
            });
        }

        public void ResetAnimation()
        {
            PlayAnimation("reset");
            safetyHarness.SetActive(false);
            animator.SetBool("reset", false);
            animator.SetBool("unconscious", false);
            animator.SetBool("pull up", false);
            npc.SetBool("unconscious", false);
            npc.SetBool("pull up", false);
            npc.SetBool("look", true);
            npc.SetForcePosition(true);
            npc.SetForceRotation(true);
            SwitchCollider(true);
        }

        public void SwitchCollider(bool value)
        {
            foreach (var c in colliders)
            {
                c.enabled = value;
            }

            unconsciousCollider.SetActive(!value);
        }
    }
}
