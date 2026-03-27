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

        public void RestartStep()
        {
            currentStep = 0;
        }
        public void StartStep()
        {
            ActiveStep(currentStep);
        }
        public void NextStep()
        {
            currentStep++;
            if (currentStep == steps.Length)
            {
                EndCheckpoint();
            }
            else
            {
                ActiveStep(currentStep);
            }
        }
        public void EndCheckpoint()
        {
            gameplayController.NextCheckpoint();
        }
        private void ActiveStep(int index)
        {
            //Debug.Log(index);
            if(steps[index].Trigger != null) steps[index].Trigger.SetActive(true);
            if (steps[index].hudPanels.Length == 1)
            {
                steps[index].hudPanels[0].panel.SetActive(true);
                if (steps[index].hudPanels[0].duration != -1)
                {
                    delay?.Kill();
                    delay = DOVirtual.DelayedCall(steps[index].hudPanels[0].duration, () =>
                    {
                        steps[index].hudPanels[0].panel.SetActive(false);
                        if (steps[index].Trigger == null) NextStep();
                    });
                }
            }
            else if (steps[index].hudPanels.Length > 1)
            {
                //Debug.Log(steps[index].hudPanels.Length);
                float firstDuration = steps[index].hudPanels[0].duration;
                float secDuration = steps[index].hudPanels[1].duration;

                delay?.Kill();
                if (secDuration != -1)
                {
                    //Debug.Log(secDuration);
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
                    //Debug.Log(secDuration);
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
