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
        [SerializeField] private GameObject checkActiveCorrectCheckpoint3;
        [SerializeField] private GameObject checkActiveFailCheckpoint3;

        [SerializeField] private GameObject accidentGuy;

        private GameplayController gameplayController;
        private Tween delay = null;
        private int currentSafeArea;
        private bool checkPoint3Done;


        private void Awake()
        {
            gameplayController = GetComponent<GameplayController>();

            GameplayController.OnCheckpointEnd += HandleCheckpointEnd;
        }
        private void OnDestroy()
        {
            GameplayController.OnCheckpointEnd -= HandleCheckpointEnd;
        }
        private void Update()
        {
            if (!checkPoint3Done)
            {
                if (checkActiveCorrectCheckpoint3.activeSelf)
                {
                    checkPoint3Done = true;
                    if (gameplayController.scoreList.Count == 2) gameplayController.scoreList.Add(true);
                    return;
                }
                if (checkActiveFailCheckpoint3.activeSelf)
                {
                    checkPoint3Done = true;
                    if (gameplayController.scoreList.Count == 2) gameplayController.scoreList.Add(false);
                    return;
                }
            }
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
            if (gameplayController.scoreList.Count == 0) gameplayController.scoreList.Add(false);
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
                    if (gameplayController.scoreList.Count == 0) gameplayController.scoreList.Add(true);
                    gameplayController.NextStep();
                    endCheckpoint1.SetActive(false);
                }
            });
        }
        public void ButtonWarning(int num)
        {
            if (num == 0)
            {
                if (gameplayController.scoreList.Count == 1) gameplayController.scoreList.Add(true);
                quizCheckpoint2.SetActive(false);
                gameplayController.NextStep();
            }
            else
            {
                if (gameplayController.scoreList.Count == 1) gameplayController.scoreList.Add(false);
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
        public void AmbulanceActive()
        {
            delay?.Kill();
            delay = DOVirtual.DelayedCall(4, () =>
            {
                accidentGuy.SetActive(false);
            });
        }
    }
}
