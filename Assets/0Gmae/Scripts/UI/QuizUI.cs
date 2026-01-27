using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Boy
{
    public class QuizUI : MonoBehaviour
    {
        public float hudDisplayDuration = 2f;
        public GameObject baseUI;
        public QuizButtonHUD[] quizButtons;

        private Tween hideHudTween;

        private void Awake()
        {
            baseUI.SetActive(true);

            foreach (var item in quizButtons)
            {
                if (item?.button == null || item.hudPanel == null) continue;

                item.hudPanel.SetActive(false);
                var hud = item.hudPanel;
                item.button.onClick.AddListener(() => ShowHud(hud));
            }
        }

        private void OnEnable()
        {
            baseUI.SetActive(true);
            HideAllHuds();
        }

        private void OnDisable()
        {
            HideAllHuds();
        }

        public void ShowHud(GameObject hudPanel)
        {
            hideHudTween?.Kill();
            HideAllHuds();
            baseUI.SetActive(false);
            hudPanel.SetActive(true);
            hideHudTween = DOVirtual
                .DelayedCall(hudDisplayDuration, () =>
                {
                    HideAllHuds();
                    baseUI.SetActive(true);
                })
                .SetLink(gameObject);
        }

        public void HideAllHuds()
        {
            foreach (var item in quizButtons)
            {
                if (item?.hudPanel == null) continue;
                item.hudPanel.SetActive(false);
            }
        }
    }
}

[Serializable]
public class QuizButtonHUD
{
    public Button button;
    public GameObject hudPanel;
}