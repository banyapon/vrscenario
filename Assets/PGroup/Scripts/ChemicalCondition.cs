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
        [SerializeField] private Boy.PPESelector pPESelector;

        private GameplayController gameplayController; 
        private Tween delay = null;

        private void Awake()
        {
            gameplayController = GetComponent<GameplayController>();

            pPESelector.OnSelectionValidated += OnValidated;
        }

        private void OnValidated(bool value)
        {
            if (value)
            {
                EndCheckpoint2();
            }
        }

        public void QuizCheckpoint1(int num)
        {
            if (num == 0)
            {
                quizCheckpoint1.SetActive(false);
                gameplayController.NextStep();
            }
            else
            {
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
            endCheckpoint2.SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                endCheckpoint2.SetActive(false);
                gameplayController.NextCheckpoint();
            });
        }
    }
}
