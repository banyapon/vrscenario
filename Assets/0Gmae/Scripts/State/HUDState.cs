using DG.Tweening;
using System;
using UnityEngine;

namespace Boy
{
    public class HUDState : MonoBehaviour
    {
        public float hudDuration = 5;
        public GameObject[] hudArray;

        State state;

        private void Awake()
        {
            state = GetComponent<State>();
            if (state == null) return;

            state.onEnter += OnEnter;
            state.onExit += OnExit;
        }
        private void OnDestroy()
        {
            if (state == null) return;
            state.onEnter -= OnEnter;
            state.onExit -= OnExit;
        }

        void OnEnter()
        {
            HideHUD();
            hudTween?.Kill();
        }

        void OnExit()
        {
            hudTween?.Kill();
        }

        Tween hudTween = null;
        public void OpenHud(GameObject hud, Action onHide = null)
        {
            hudTween?.Kill();
            HideHUD();
            if (hud != null) hud.SetActive(true);
            hudTween = DOVirtual.DelayedCall(hudDuration, HideHUD)
                .SetLink(gameObject).OnComplete(() => { onHide?.Invoke(); });
        }
        public void OpenHud(GameObject hud, float duration, Action onHide = null)
        {
            hudTween?.Kill();
            HideHUD();
            if (hud != null) hud.SetActive(true);
            hudTween = DOVirtual.DelayedCall(duration, HideHUD)
                .SetLink(gameObject).OnComplete(() => { onHide?.Invoke(); });
        }
        public void HideHUD()
        {
            foreach (var hud in hudArray)
            {
                if (hud == null) continue;
                hud.SetActive(false);
            }
        }
    }
}
