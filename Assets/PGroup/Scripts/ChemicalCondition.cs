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
        [SerializeField] private GameObject warningSiren;

        [Header("Restart")]
        [SerializeField] private GameObject[] activeObjRestart;
        [SerializeField] private GameObject[] deactiveObjRestart;
        [SerializeField] private Transform[] resetPositionObjRestart;

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
            warningSiren.SetActive(false);
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
        public void ButtonPlayAgain()
        {
            checkPoint3Done = false;
            gameplayController.RestartCheckpoint();


            resetPositionObjRestart[0].localPosition = new Vector3(-7.83135939f, 0.297994792f, 0.89168787f);
            resetPositionObjRestart[1].localPosition = new Vector3(-7.83135939f, 0.333000004f, 0.89168787f);
            resetPositionObjRestart[2].localPosition = new Vector3(-7.83135939f, 0.372999996f, 0.89168787f);
            resetPositionObjRestart[3].localPosition = new Vector3(-3.34800005f, 0.972000003f, 1.38499999f);
            resetPositionObjRestart[4].localPosition = new Vector3(-3.03299999f, 0.972000003f, 1.38499999f);
            resetPositionObjRestart[5].localPosition = new Vector3(-2.70600009f, 0.972000003f, 1.38499999f);
            resetPositionObjRestart[6].localPosition = new Vector3(-7.60410929f, 1.12650287f, 0.17592144f);
            resetPositionObjRestart[0].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[1].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[2].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[3].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[4].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[5].localEulerAngles = Vector3.zero;
            resetPositionObjRestart[6].localEulerAngles = new Vector3(0, 238.652206f, 0);
            resetPositionObjRestart[3].SetParent(resetPositionObjRestart[7]);
            resetPositionObjRestart[4].SetParent(resetPositionObjRestart[7]);
            resetPositionObjRestart[5].SetParent(resetPositionObjRestart[7]);


            for (int i = 0; i < activeObjRestart.Length; i++)
            {
                activeObjRestart[i].SetActive(true);
            }
            for (int i = 0; i < deactiveObjRestart.Length; i++)
            {
                deactiveObjRestart[i].SetActive(false);
            }

        }
    }
}
