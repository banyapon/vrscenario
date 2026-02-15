using System;
using DG.Tweening;
using UnityEngine;

namespace PGroup
{
    public class ConstructionSiteCondition : MonoBehaviour
    {
        [SerializeField] private GameObject[] correctSafeArea;
        [SerializeField] private GameObject[] quizSafeArea;
        [SerializeField] private GameObject wrongSafeArea;
        [SerializeField] private GameObject endCheckpoint1;
        [SerializeField] private GameObject[] checkMarks;
        [SerializeField] private GameObject paper;

        [SerializeField] private GameObject quizCheckpoint2;
        [SerializeField] private GameObject wrongWarning;

        [SerializeField] private GameObject startAnimationCheckpoint3;
        [SerializeField] private GameObject deactiveBeforeAnimationCheckpoint3;

        private GameplayController gameplayController;
        private Tween delay = null;
        private int currentSafeArea;


        private void Awake()
        {
            gameplayController = GetComponent<GameplayController>();

            GameplayController.OnCheckpointEnd += HandleCheckpointEnd;
        }
        private void OnDestroy()
        {
            GameplayController.OnCheckpointEnd -= HandleCheckpointEnd;
        }

        private void HandleCheckpointEnd(int num)
        {
            Debug.Log(num);
            if(num == 2)
            {
                deactiveBeforeAnimationCheckpoint3.SetActive(false);
                startAnimationCheckpoint3.SetActive(true);
            }
        }

        public void ButtonSafeArea(int num)
        {
            if (currentSafeArea >= 3) return;
            switch (num)
            {
                case 0: CorrectSafeArea(num); break;
                case 1: CorrectSafeArea(num); break;
                case 2: CorrectSafeArea(num); break;
                case 3: WrongSafeArea(num); break;
                case 4: WrongSafeArea(num); break;
            }
            quizSafeArea[num].SetActive(false);
        }
        private void WrongSafeArea(int num)
        {
            delay?.Kill();
            delay = DOTween.Sequence()
                .AppendCallback(() => wrongSafeArea.SetActive(true))
                .AppendInterval(3)
                .AppendCallback(() =>
                {
                    wrongSafeArea.SetActive(false);
                    paper.SetActive(true);
                })
                .AppendInterval(1)
                .AppendCallback(() =>
                {
                    checkMarks[num].SetActive(true);
                })
                .AppendInterval(3)
                .AppendCallback(() =>
                {
                    paper.SetActive(false);
                });
        }
        private void CorrectSafeArea(int num)
        {
            currentSafeArea++;
            correctSafeArea[num].SetActive(true);
            paper.SetActive(true);
            checkMarks[num].SetActive(true);
            if (currentSafeArea == 3)
            {
                for (int i = 0; i < checkMarks.Length; i++)
                {
                    checkMarks[i].SetActive(true);
                }
            }
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                paper.SetActive(false);
                if (currentSafeArea == 3)
                {
                    gameplayController.NextStep();
                    endCheckpoint1.SetActive(false);
                }
            });
        }
        public void ButtonWarning(int num)
        {
            if (num == 0)
            {
                quizCheckpoint2.SetActive(false);
                gameplayController.NextStep();
            }
            else
            {
                quizCheckpoint2.SetActive(false);
                wrongWarning.SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(3, () =>
                {
                    quizCheckpoint2.SetActive(true);
                    wrongWarning.SetActive(false);
                });
            }
        }
    }
}
