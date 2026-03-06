using DG.Tweening;
using System;
using UnityEngine;

namespace PGroup
{
    public class ChemicalCondition : MonoBehaviour
    {
        [SerializeField] private GameObject quizCheckpoint1;
        [SerializeField] private GameObject failCheckpoint1;
        [SerializeField] private GameObject endCheckpoint1;
        [SerializeField] private GameObject endCheckpoint2;
        [SerializeField] private GameObject quizCheckpoint4;
        [SerializeField] private GameObject failCheckpoint4;
        [SerializeField] private Boy.PPESelector pPESelector;
        [SerializeField] private GameObject checkActiveCorrectCheckpoint3;
        [SerializeField] private GameObject checkActiveFailCheckpoint3;

        private GameplayController gameplayController; 
        private Tween delay = null;
        private bool checkPoint3Done;

        private void Awake()
        {
            gameplayController = GetComponent<GameplayController>();

            pPESelector.OnSelectionValidated += OnValidated;
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
        private void OnValidated(bool value)
        {
            if (value)
            {
                if (gameplayController.scoreList.Count == 1) gameplayController.scoreList.Add(true);
                EndCheckpoint2();
            }
            else
            {
                if (gameplayController.scoreList.Count == 1) gameplayController.scoreList.Add(false);
            }
        }

        public void QuizCheckpoint1(int num)
        {
            if (num == 0)
            {
                if (gameplayController.scoreList.Count == 0) gameplayController.scoreList.Add(true);
                quizCheckpoint1.SetActive(false);
                gameplayController.NextStep();
            }
            else
            {
                if (gameplayController.scoreList.Count == 0) gameplayController.scoreList.Add(false);
                quizCheckpoint1.SetActive(false);
                failCheckpoint1.SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(3, () =>
                {
                    quizCheckpoint1.SetActive(true);
                    failCheckpoint1.SetActive(false);
                });
            }
        }
        public void EndCheckpoint1()
        {
            endCheckpoint1.SetActive(false);
            gameplayController.NextCheckpoint();
        }
        public void EndCheckpoint2()
        {
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                endCheckpoint2.SetActive(false);
                gameplayController.NextCheckpoint();
            });
        }
        public void QuizCheckpoint4(int num)
        {
            if (num == 0)
            {
                if (gameplayController.scoreList.Count == 3) gameplayController.scoreList.Add(true);
                quizCheckpoint4.SetActive(false);
                gameplayController.NextStep();
            }
            else
            {
                if (gameplayController.scoreList.Count == 3) gameplayController.scoreList.Add(false);
                quizCheckpoint4.SetActive(false);
                failCheckpoint4.SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(3, () =>
                {
                    quizCheckpoint4.SetActive(true);
                    failCheckpoint4.SetActive(false);
                });
            }
        }
    }
}
