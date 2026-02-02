using DG.Tweening;
using System;
using UnityEngine;

namespace Boy
{
    public class MachineDoor : MonoBehaviour
    {
        [Header("Setting")]
        public float duraion = 3;
        public float moveDuration = 0.35f;
        public Ease moveEase = Ease.OutCubic;

        [Header("Reference")]
        public Transform door;
        public Transform openTrans;

        TriggerChecker checker;
        Tween doorTween;

        private void Awake()
        {
            checker = GetComponent<TriggerChecker>();
            checker.OnEnter += HandleOpen;
        }

        public void Open(Action callback = null)
        {
            doorTween?.Kill();
            doorTween = door.DOLocalMove(openTrans.localPosition, moveDuration)
                .SetEase(moveEase).SetLink(gameObject)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        public void Close(Action callback = null)
        {
            doorTween?.Kill();
            doorTween = door.DOLocalMove(Vector3.zero, moveDuration)
                .SetEase(moveEase).SetLink(gameObject)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }
        private void OnDestroy()
        {
            if (checker) checker.OnEnter -= HandleOpen;
        }

        private void HandleOpen()
        {
            checker.enabled = false;

            Open(() =>
            {
                DOVirtual.DelayedCall(duraion, HandleClose)
                .SetLink(gameObject);
            });
        }
        private void HandleClose()
        {
            Close(() =>
            {
                checker.enabled = true;
            });
        }
    }
}
