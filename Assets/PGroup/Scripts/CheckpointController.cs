using DG.Tweening;
using System;
using UnityEngine;

namespace PGroup
{
    public class CheckpointController : MonoBehaviour
    {
        [SerializeField] private Step[] steps;


        private int currentStep;
        private Tween delay = null;
        private GameplayController gameplayController;

        private void Awake()
        {
            gameplayController = GetComponentInParent<GameplayController>();
        }

        public void StartStep()
        {
            ActiveStep(currentStep);
        }
        public void NextStep()
        {
            currentStep++;
            ActiveStep(currentStep);
        }
        public void EndCheckpoint()
        {
            gameplayController.NextCheckpoint();
        }
        private void ActiveStep(int index)
        {
            if(steps[index].Trigger != null) steps[index].Trigger.SetActive(true);
            if(steps[index].hudPanels.Length > 0)
            {
                steps[index].hudPanels[0].panel.SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(steps[index].hudPanels[0].duration, () =>
                {
                    steps[index].hudPanels[0].panel.SetActive(false);
                });
            }
            else if (steps[index].hudPanels.Length > 1)
            {
                float firstDuration = steps[index].hudPanels[0].duration;
                float secDuration = steps[index].hudPanels[1].duration;

                delay?.Kill();
                if (secDuration != -1)
                {
                    delay = DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            steps[index].hudPanels[0].panel.SetActive(true);
                        })
                        .AppendInterval(steps[index].hudPanels[0].duration)
                        .AppendCallback(() =>
                        {
                            steps[index].hudPanels[0].panel.SetActive(false);
                            steps[index].hudPanels[1].panel.SetActive(true);
                        })
                        .AppendInterval(steps[index].hudPanels[1].duration)
                        .AppendCallback(() =>
                        {
                            steps[index].hudPanels[1].panel.SetActive(false);
                        });
                }
                else
                {
                    delay = DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            steps[index].hudPanels[0].panel.SetActive(true);
                        })
                        .AppendInterval(steps[index].hudPanels[0].duration)
                        .AppendCallback(() =>
                        {
                            steps[index].hudPanels[0].panel.SetActive(false);
                            steps[index].hudPanels[1].panel.SetActive(true);
                        });
                }
            }
        }
    }
    [Serializable]
    public class Step
    {
        public GameObject Trigger;
        public HUD[] hudPanels;
    }
    [Serializable]
    public class HUD
    {
        public GameObject panel;
        public float duration;
    }
}
