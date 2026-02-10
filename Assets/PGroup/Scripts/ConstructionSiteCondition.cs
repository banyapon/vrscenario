using DG.Tweening;
using UnityEngine;

namespace PGroup
{
    public class ConstructionSiteCondition : MonoBehaviour
    {
        [SerializeField] private GameObject[] correctSafeArea;
        [SerializeField] private GameObject wrongSafeArea;
        [SerializeField] private GameObject endCheckpoint1;

        [SerializeField] private GameObject quizCheckpoint2;
        [SerializeField] private GameObject wrongWarning;

        private GameplayController gameplayController;
        private Tween delay = null;
        private int currentSafeArea;

        private void Awake()
        {
            gameplayController = GetComponent<GameplayController>();
        }
        public void ButtonSafeArea(int num)
        {
            if (currentSafeArea >= 3) return;
            switch (num)
            {
                case 0: CorrectSafeArea(num); break;
                case 1: CorrectSafeArea(num); break;
                case 2: CorrectSafeArea(num); break;
                case 3: WrongSafeArea(); break;
                case 4: WrongSafeArea(); break;
            }
        }
        private void WrongSafeArea()
        {
            wrongSafeArea.SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                wrongSafeArea.SetActive(false);
            });
        }
        private void CorrectSafeArea(int num)
        {
            currentSafeArea++;
            correctSafeArea[num].SetActive(true);
            if (currentSafeArea == 3)
            {
                gameplayController.NextStep();
                endCheckpoint1.SetActive(false);
            }
        }
        public void ButtonWarning(int num)
        {
            if (num == 0)
            {
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
