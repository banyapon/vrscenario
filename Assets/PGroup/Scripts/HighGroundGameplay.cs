using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class HighGroundGameplay : MonoBehaviour
    {
        [SerializeField] private Transform player;

        [Header("CheckPoint 1")]
        [SerializeField] private GameObject[] uiCheckpoint1;
        [SerializeField] private GameObject[] hlCheckpoint1;
        [SerializeField] private Boy.PPESelector pPESelector;

        [Header("CheckPoint 2")]
        [SerializeField] private GameObject[] uiCheckpoint2;
        [SerializeField] private GameObject[] hlCheckpoint2;
        [SerializeField] private TriggerChecker point1;
        [SerializeField] private Hook hookLeft;
        [SerializeField] private Hook hookRight;
        [SerializeField] private TriggerChecker[] ladders;
        [SerializeField] private Transform blockUp;
        [SerializeField] private Transform blockDown;
        private bool isHookOnL;
        private bool isHookOnR;
        private int currentLadderHook;
        [Header("CheckPoint 3")]
        [SerializeField] private GameObject[] uiCheckpoint3;
        [SerializeField] private GameObject[] hlCheckpoint3;
        [Header("CheckPoint 4")]
        [SerializeField] private GameObject[] uiCheckpoint4;
        [SerializeField] private GameObject[] hlCheckpoint4;
        [Header("CheckPoint 5")]
        [SerializeField] private GameObject[] uiCheckpoint5;
        [SerializeField] private GameObject[] hlCheckpoint5;
        [Header("CheckPoint 6")]
        [SerializeField] private GameObject[] uiCheckpoint6;
        [SerializeField] private GameObject[] hlCheckpoint6;
        [Header("CheckPoint 7")]
        [SerializeField] private GameObject[] uiCheckpoint7;
        [SerializeField] private GameObject[] hlCheckpoint7;


        private Tween delay = null;
        private void Awake()
        {
            pPESelector.OnSelectionValidated += OnValidated;
            point1.OnEnter += () => Checkpoint2Start();
            hookLeft.OnEnter += () => SetHookOn("Left", true);
            hookLeft.OnExit += () => SetHookOn("Left", false);
            hookRight.OnEnter += () => SetHookOn("Right", true);
            hookRight.OnExit += () => SetHookOn("Right", false);
            foreach (var item in ladders)
            {
                item.OnEnter += () => OnTryClimb();
            }
            player = Camera.main.transform;
        }

        private void Start()
        {
            Checkpoint1Start();

            ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(true);
        }
        #region Checkpoint 1
        private void Checkpoint1Start()
        {
            delay?.Kill();
            delay = DOTween.Sequence()
                .AppendCallback(() => uiCheckpoint1[0].SetActive(true))
                .AppendInterval(2)
                .AppendCallback(() =>
                {
                    uiCheckpoint1[0].SetActive(false);
                    uiCheckpoint1[1].SetActive(true);
                })
                .AppendInterval(5)
                .AppendCallback(() =>
                {
                    uiCheckpoint1[1].SetActive(false);
                    uiCheckpoint1[2].SetActive(true);
                });
        }

        private void OnValidated(bool value)
        {
            if (!value) return;
            uiCheckpoint1[2].SetActive(false);
            uiCheckpoint1[3].SetActive(true);
            uiCheckpoint1[4].SetActive(true);
            hlCheckpoint1[0].SetActive(true);
        }
        public void Checkpoint1Success()
        {
            hlCheckpoint1[0].SetActive(false);
            uiCheckpoint1[3].SetActive(false);
            uiCheckpoint1[4].SetActive(false);
            uiCheckpoint1[5].SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(2, () =>
            {
                uiCheckpoint1[5].SetActive(false);
                uiCheckpoint2[0].SetActive(true);
            });
        }
        #endregion
        #region Checkpoint 2
        private void Checkpoint2Start()
        {
            uiCheckpoint2[0].SetActive(false);
            uiCheckpoint2[1].SetActive(true);
        }
        private void OnTryClimb()
        {
            if (!IsHookOn())
            {
                uiCheckpoint2[2].transform.position = new Vector3(uiCheckpoint2[2].transform.position.x, player.position.y, uiCheckpoint2[2].transform.position.z);
                uiCheckpoint2[2].SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(2, () =>
                {
                    uiCheckpoint2[2].SetActive(false);
                });
            }
        }
        private bool IsHookOn()
        {
            if (isHookOnL && isHookOnR) return true;
            else return false;
        }
        private void SetHookOn(string side, bool isAttach)
        {
            Hook getHook;
            if (side == "Left")
            {
                isHookOnL = isAttach;
                getHook = hookLeft;
            }
            else
            {
                isHookOnR = isAttach;
                getHook = hookRight;
            }

            if (isAttach) SetBlocker(getHook);
        }
        public void SetBlocker(Hook hookSide)
        {
            if (hookSide == hookLeft)
            {
                hookLeft.GetComponent<XRGrabInteractable>().enabled = false;
                hookRight.GetComponent<XRGrabInteractable>().enabled = true;
            }
            else
            {
                hookRight.GetComponent<XRGrabInteractable>().enabled = false;
                hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
            }
            hookSide.transform.position = ladders[currentLadderHook].transform.GetChild(0).position;
            hookSide.transform.rotation = ladders[currentLadderHook].transform.GetChild(0).rotation;
            ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(false);
            currentLadderHook++;
            ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(true);
            blockUp.position = new Vector3(blockUp.position.x, hookSide.hitObject.transform.position.y + 1, blockUp.position.z);

        }
        #endregion
        #region Checkpoint 3
        #endregion
        #region Checkpoint 4
        #endregion
        #region Checkpoint 5
        #endregion
        #region Checkpoint 6
        #endregion
        #region Checkpoint 7
        #endregion
    }
}
