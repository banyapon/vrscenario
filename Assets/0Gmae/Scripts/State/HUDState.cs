using DG.Tweening;
using UnityEngine;

namespace Boy
{
    public class HUDState : MonoBehaviour
    {
        public float hudDuration = 2;
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
        public void OpenHud(GameObject hud)
        {
            hudTween?.Kill();
            HideHUD();
            hud.SetActive(true);
            hudTween = DOVirtual.DelayedCall(hudDuration, HideHUD).SetLink(gameObject);
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
