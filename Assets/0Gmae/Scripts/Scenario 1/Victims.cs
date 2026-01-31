using DG.Tweening;
using System;
using UnityEngine;

namespace Boy
{
    public class Victims : MonoBehaviour
    {
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
            PlayAnimation("unconscious", unconsciousClip.length, callback);
        }

        public void Pullup(Action callback = null)
        {
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
        }
    }
}
