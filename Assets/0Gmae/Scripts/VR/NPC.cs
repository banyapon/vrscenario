using DG.Tweening;
using UnityEngine;

namespace Boy
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] bool forcePosition = false;
        [SerializeField] bool forceRotation = false;
        [SerializeField] Animator animator;
        [SerializeField] Transform model;

        private void Update()
        {
            if (model == null) return;
            if (forcePosition) model.localPosition = Vector3.zero;
            if (forceRotation) model.localEulerAngles = Vector3.zero;
        }

        public void SetFloat(string parameter, float value)
        {
            animator.SetFloat(parameter, value);
        }

        public float GetFloat(string parameter)
        {
            return animator.GetFloat(parameter);
        }

        public void SetBool(string parameter, bool value)
        {
            animator.SetBool(parameter, value);
        }

        Tween posTween;
        public void SetForcePosition(bool value, float delay = 0.5f)
        {
            posTween?.Kill();
            posTween = DOVirtual.DelayedCall(delay, () => { forcePosition = value; });
        }

        Tween rotateTween;
        public void SetForceRotation(bool value, float delay = 0.5f)
        {
            rotateTween?.Kill();
            rotateTween = DOVirtual.DelayedCall(delay, () => { forceRotation = value; });
        }
    }
}
