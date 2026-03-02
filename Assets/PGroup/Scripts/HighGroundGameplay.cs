using Boy;
using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PGroup
{
    public class HighGroundGameplay : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform positionHookLeft;
        [SerializeField] private Transform positionHookRight;
        [SerializeField] private Transform positionEndgame;
        [SerializeField] private SummaryUI summaryUI;
        [SerializeField] private Transform endRope;
        [SerializeField] private GameObject ladder;
        private List<bool> scoreList;
        private int score;
        private float timeUsed;
        private bool onPlaying;

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
        [SerializeField] private GameObject rope;
        [SerializeField] private TriggerChecker[] ladders;
        [SerializeField] private Transform blockUp;
        [SerializeField] private Transform blockDown;
        private bool isHookOnL;
        private bool isHookOnR;
        private int currentLadderHook;
        private bool onCheckClimbing;
        [Header("CheckPoint 3")]
        [SerializeField] private GameObject[] uiCheckpoint3;
        [SerializeField] private GameObject[] hlCheckpoint3;
        [SerializeField] private TriggerChecker startPoint3;
        [SerializeField] private TriggerChecker startPoint3_2;
        [SerializeField] private TriggerChecker startPoint3_3;
        [SerializeField] private TriggerChecker startPoint3_4;
        [SerializeField] private PlaceObject slingTop1;
        [SerializeField] private PlaceObject slingTop2;
        [SerializeField] private PlaceObject slingTop3;
        [SerializeField] private PlaceObject slingTop4;
        [SerializeField] private PlaceObject slingTop5;
        [SerializeField] private PlaceObject slingTop6;
        private PlaceObject processTrigger;
        [Header("CheckPoint 4")]
        [SerializeField] private GameObject[] uiCheckpoint4;
        [SerializeField] private GameObject[] hlCheckpoint4;
        [SerializeField] private TriggerChecker movePoint4;
        [SerializeField] private TriggerChecker movePoint4_2;
        [SerializeField] private Animation npcAnim;
        private bool isClimbDown;
        [Header("CheckPoint 5")]
        [SerializeField] private GameObject[] uiCheckpoint5;
        [SerializeField] private GameObject[] hlCheckpoint5;
        [Header("CheckPoint 6")]
        [SerializeField] private GameObject[] uiCheckpoint6;
        [SerializeField] private GameObject[] hlCheckpoint6;
        [Header("CheckPoint 7")]
        [SerializeField] private GameObject[] uiCheckpoint7;
        [SerializeField] private GameObject[] hlCheckpoint7;

        [Header("Thermalscan")]
        [SerializeField] private GameObject[] scanArea;
        [SerializeField] private GameObject[] scanCompleted;
        [SerializeField] private GameObject[] thermalscanProcess;
        [SerializeField] private Thermalscan thermalscan;
        private int scanPoint;


        private Tween delay = null;
        Scenario scenario;
        private void Awake()
        {
            scenario = GetComponentInParent<Scenario>();
            player = Camera.main.transform;
            endRope.parent = player.parent;
            pPESelector.OnSelectionValidated += OnValidated;
            point1.OnEnter += () => Checkpoint2Start();
            startPoint3.OnEnter += () => Checkpoint3Start();
            startPoint3_2.OnEnter += () => ChangeSling();
            startPoint3_3.OnEnter += () => Thermalscan();
            startPoint3_4.OnEnter += () => CheckPoint3Success();
            movePoint4.OnEnter += () => Checkpoint4Start();
            movePoint4_2.OnEnter += () => Accident();
            hookLeft.OnEnter += OnHookHit; 
            //hookLeft.OnExit += () => SetHookOn("Left", false);
            hookRight.OnEnter += OnHookHit;
            //hookRight.OnExit += () => SetHookOn("Right", false);
            thermalscan.OnEnter += OnThermalscanEnter;
            foreach (var item in ladders)
            {
                item.OnEnter += () => OnTryClimb();
            }

            //Checkpoint 3
            /*slingTop1.OnEnter += GetTrigger;
            slingTop2.OnEnter += GetTrigger;
            slingTop3.OnEnter += GetTrigger;
            slingTop4.OnEnter += GetTrigger;
            slingTop5.OnEnter += GetTrigger;
            slingTop6.OnEnter += GetTrigger;*/

            //SetScore
            scoreList = new List<bool>(4);

            for (int i = 0; i < 4; i++)
            {
                scoreList.Add(false);
            }

            Debug.Log(scoreList.Count);
        }

        private void OnThermalscanEnter(GameObject thermal, GameObject hit)
        {
            for (int i = 0; i < scanArea.Length; i++)
            {
                if (hit == scanArea[i])
                {
                    if (hit.activeSelf)
                    {
                        scanPoint++;
                        hit.gameObject.SetActive(false);
                        scanCompleted[i].SetActive(true);
                        delay?.Kill();
                        delay = DOVirtual.DelayedCall(2, () =>
                        {
                            CheckScanPoint();
                        });
                    }
                }
            }
        }
        private void CheckScanPoint()
        {
            if (scanPoint >= 3)
            {
                for (int i = 0; i < thermalscanProcess.Length; i++)
                {
                    thermalscanProcess[i].gameObject.SetActive(false);
                }
                CheckPoint3Back();
            }
        }

        private void Start()
        {
            Checkpoint1Start();
        }
        private void Update()
        {
            if (onPlaying)
                timeUsed += Time.deltaTime;
        }
        private void OnHookHit(GameObject hook,GameObject hitObject)
        {
            if (hitObject.CompareTag("Ladder"))
            {
                if (hook == hookLeft.gameObject)
                {
                    SetHookOn("Left", true);
                }
                else
                {
                    SetHookOn("Right", true);
                }
            }
            if (hitObject.GetComponent<PlaceObject>())
            {
                GetTrigger(hitObject.GetComponent<PlaceObject>(), hook);
            }
        }
        private void ShowResult()
        {
            onPlaying = false;
            ladder.SetActive(false);
            rope.SetActive(false);
            hookLeft.gameObject.SetActive(false);
            hookRight.gameObject.SetActive(false);
            /*scoreList.Add(true);
            scoreList.Add(true);
            scoreList.Add(true);
            scoreList.Add(true);*/
            if (scenario) Player.Instance?.Teleport(positionEndgame.position, Vector3.zero, scenario.IsOwner);
            summaryUI.gameObject.SetActive(true);
            summaryUI.ShowSummary(scoreList);

            SendScoreAPI();
        }
        private void PlayAnimation(Animation animation, string clip, bool reversed)
        {
            if (!reversed)
            {
                animation[clip].speed = 1f;
                animation[clip].time = 0;
                animation.PlayQueued(clip);
            }
            else
            {
                animation[clip].speed = -1f;
                animation[clip].time = animation[clip].length;
                animation.Play(clip);
            }
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

            //hookLeft.transform.position = player.parent.position + positionHookLeft.position;
            //hookRight.transform.position = player.parent.position + positionHookRight.position;

            rope.gameObject.SetActive(true);
            hookLeft.gameObject.SetActive(true);
            hookRight.gameObject.SetActive(true);
            //hookLeft.GetComponent<Rigidbody>().isKinematic = true;
            //hookRight.GetComponent<Rigidbody>().isKinematic = true;
            //hlCheckpoint1[0].SetActive(true);

            //GetScore
            scoreList[0] = true;
        }
        public void Checkpoint1Success()
        {
            Debug.Log("Checkpoint 1 Success");
            //hlCheckpoint1[0].SetActive(false);
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
            point1.gameObject.SetActive(false);
            onCheckClimbing = true;
            ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(true);
            uiCheckpoint2[0].SetActive(false);
            uiCheckpoint2[1].SetActive(true);
        }
        private void OnTryClimb()
        {
            Debug.Log("Check : " + IsHookOn());
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
            else
            {
                uiCheckpoint2[1].SetActive(false);
                uiCheckpoint2[2].SetActive(false);
            }
        }
        private bool IsHookOn()
        {
            if (isHookOnL && isHookOnR) return true;
            else return false;
        }
        private void SetHookOn(string side, bool isAttach)
        {
            if (!onCheckClimbing) return;
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
            //ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(false);
            Debug.Log(currentLadderHook);

            bool condition;
            if (!isClimbDown)
            {
                condition = currentLadderHook < ladders.Length - 3;
                if (condition) currentLadderHook++;
            }
            else
            {
                condition = currentLadderHook > 0;
                if (condition) currentLadderHook--;
            }
            if (condition)
            {
                ladders[currentLadderHook].transform.GetChild(0).gameObject.SetActive(true);
                blockUp.position = new Vector3(blockUp.position.x, hookSide.hitObject.transform.position.y + 2, blockUp.position.z);
                blockDown.position = new Vector3(blockDown.position.x, hookSide.hitObject.transform.position.y - 2, blockDown.position.z);
            }
            else
            {
                onCheckClimbing = false;
                uiCheckpoint3[0].SetActive(true);
                //hookRight.GetComponent<XRGrabInteractable>().enabled = true;
                //hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
                blockDown.position = new Vector3(blockDown.position.x, hookSide.hitObject.transform.position.y - 2, blockDown.position.z);
            }

            //GetScore
            scoreList[1] = true;
        }
        private void SendScoreAPI()
        {
            //SCORE
            int score = 0;
            for (int i = 0; i < scoreList.Count; i++)
            {
                if (scoreList[i]) score++;
            }
            //STAR
            int star = Convert.ToInt32(score / (float)scoreList.Count);

            Debug.Log(star);

            var body = new
            {
                userEmail = APIManager.Instance.userEmail,
                scenarioKey = "scenario2",
                total_score = score,
                stars = star,
                details = new
                {
                    ppe_work_permit = scoreList[0],
                    climbing = scoreList[1],
                    anchor_point = scoreList[2],
                    emergency_call = scoreList[3],
                },
                time_used_seconds = (int)timeUsed,
                remark = ""
            };

            string json = JsonConvert.SerializeObject(body);
            print(json);

            APIManager.Instance.SaveSession<string>(json, (ok, msg, res) =>
            {
                print(msg);
                if (!ok) return;
                print(res);
            });
        }
        #endregion
        #region Checkpoint 3
        private void Checkpoint3Start()
        {
            startPoint3.gameObject.SetActive(false);
            uiCheckpoint3[1].SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(2, () =>
            {
                uiCheckpoint3[1].SetActive(false);
            });
            uiCheckpoint3[2].SetActive(true);
            slingTop1.gameObject.SetActive(true);
            //slingTop2.gameObject.SetActive(true);
            isHookOnR = false;
            isHookOnL = false;
            //hookLeft.transform.position = player.parent.position + positionHookLeft.position;
            //hookRight.transform.position = player.parent.position + positionHookRight.position;
            //hookRight.GetComponent<XRGrabInteractable>().enabled = true;
            //hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
            blockUp.gameObject.SetActive(false);
        }
        private void GetTrigger(PlaceObject receiver,GameObject trigger)
        {
            SlingHookTop(trigger, receiver.transform);
        }
        private void SlingHookTop(GameObject hook,Transform pos)
        {
            if (hook == hookLeft.gameObject)
            {
                hookLeft.GetComponent<XRGrabInteractable>().enabled = false;
                isHookOnL = true;
            }
            else
            {
                hookRight.GetComponent<XRGrabInteractable>().enabled = false;
                isHookOnR = true;
            }
            hook.transform.position = pos.position;
            hook.transform.rotation = pos.rotation;

            //Debug.Log(isHookOnL);
            //Debug.Log(isHookOnR);

            if (isHookOnL && isHookOnR)
            {
                uiCheckpoint3[1].SetActive(false);
                uiCheckpoint3[2].SetActive(false);
                uiCheckpoint3[3].SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(2, () =>
                {
                    uiCheckpoint3[3].SetActive(false);
                });
                if (slingTop1.gameObject.activeSelf || slingTop2.gameObject.activeSelf)
                {
                    slingTop1.gameObject.SetActive(false);
                    slingTop2.gameObject.SetActive(false);
                    startPoint3_2.gameObject.SetActive(true);
                    hookLeft.SetOffset();
                    hookRight.SetOffset();
                    hookLeft.isFollowPlayer = new Vector3(1, 0, 0);
                    hookRight.isFollowPlayer = new Vector3(1, 0, 0);
                    isHookOnR = false;
                    isHookOnL = false;

                    //GetScore
                    scoreList[2] = true;
                }
                else if (slingTop3.gameObject.activeSelf || slingTop4.gameObject.activeSelf)
                {
                    slingTop3.gameObject.SetActive(false);
                    slingTop4.gameObject.SetActive(false);
                    startPoint3_3.gameObject.SetActive(true);
                    hookLeft.SetOffset();
                    hookRight.SetOffset();
                    hookLeft.isFollowPlayer = new Vector3(0, 0, 1);
                    hookRight.isFollowPlayer = new Vector3(0, 0, 1);
                    isHookOnR = false;
                    isHookOnL = false;
                }
                else if (slingTop5.gameObject.activeSelf || slingTop6.gameObject.activeSelf)
                {
                    movePoint4.gameObject.SetActive(true);
                    isClimbDown = true;
                    //ladders[15].transform.GetChild(0).gameObject.SetActive(true);
                    slingTop5.gameObject.SetActive(false);
                    slingTop6.gameObject.SetActive(false);
                    hookLeft.SetOffset();
                    hookRight.SetOffset();
                    hookLeft.isFollowPlayer = new Vector3(1, 0, 0);
                    hookRight.isFollowPlayer = new Vector3(1, 0, 0);
                    isHookOnR = false;
                    isHookOnL = false;
                }
            }
            else
            {
                if (slingTop1.gameObject.activeSelf)
                {
                    slingTop1.gameObject.SetActive(false);
                    slingTop2.gameObject.SetActive(true);
                }
                else if (slingTop3.gameObject.activeSelf)
                {
                    slingTop3.gameObject.SetActive(false);
                    slingTop4.gameObject.SetActive(true);
                }
                else if (slingTop5.gameObject.activeSelf)
                {
                    slingTop5.gameObject.SetActive(false);
                    slingTop6.gameObject.SetActive(true);
                }
                if (hook == hookLeft.gameObject)
                {
                    hookLeft.GetComponent<XRGrabInteractable>().enabled = false;
                    hookRight.GetComponent<XRGrabInteractable>().enabled = true;
                    isHookOnL = true;
                }
                else
                {
                    hookRight.GetComponent<XRGrabInteractable>().enabled = false;
                    hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
                    isHookOnR = true;
                }
            }

            pos.gameObject.SetActive(false);
        }
        private void ChangeSling()
        {
            startPoint3_2.gameObject.SetActive(false);
            uiCheckpoint3[2].SetActive(true);
            hookLeft.isFollowPlayer = Vector3.zero;
            hookRight.isFollowPlayer = Vector3.zero;
            //hookRight.GetComponent<XRGrabInteractable>().enabled = true;
            hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
            slingTop3.gameObject.SetActive(true);
            //slingTop4.gameObject.SetActive(true);
        }
        private void Thermalscan()
        {
            startPoint3_3.gameObject.SetActive(false);
            for (int i = 0; i < thermalscanProcess.Length; i++)
            {
                thermalscanProcess[i].gameObject.SetActive(true);
            }
        }
        private void CheckPoint3Back()
        {
            startPoint3_3.gameObject.SetActive(false);
            startPoint3_4.gameObject.SetActive(true);
            uiCheckpoint3[5].SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(2, () =>
            {
                uiCheckpoint3[5].SetActive(false);
            });
        }
        private void CheckPoint3Success()
        {
            hookLeft.isFollowPlayer = new Vector3(0, 0, 0);
            hookRight.isFollowPlayer = new Vector3(0, 0, 0);
            startPoint3_4.gameObject.SetActive(false);
            slingTop5.gameObject.SetActive(true);
            //slingTop6.gameObject.SetActive(true);
            //hookRight.GetComponent<XRGrabInteractable>().enabled = true;
            hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
            blockDown.position += new Vector3(0, .7f, 0);
        }
        #endregion
        #region Checkpoint 4
        private void Checkpoint4Start()
        {
            movePoint4.gameObject.SetActive(false);
            ladders[16].transform.GetChild(0).gameObject.SetActive(true);
            hookLeft.isFollowPlayer = new Vector3(0, 0, 0);
            hookRight.isFollowPlayer = new Vector3(0, 0, 0);
            //hookLeft.transform.position = player.parent.position + positionHookLeft.position;
            //hookRight.transform.position = player.parent.position + positionHookRight.position;
            hookRight.GetComponent<XRGrabInteractable>().enabled = true;
            hookLeft.GetComponent<XRGrabInteractable>().enabled = true;
            isHookOnR = false;
            isHookOnL = false;
            blockUp.gameObject.SetActive(true);
            onCheckClimbing = true;
            movePoint4_2.gameObject.SetActive(true);
        }
        private void Accident()
        {
            movePoint4_2.gameObject.SetActive(false);
            for (int i = 0; i < ladders.Length; i++)
            {
                if (ladders[i].transform.childCount != 0)
                    ladders[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            hookRight.GetComponent<XRGrabInteractable>().enabled = false;
            hookLeft.GetComponent<XRGrabInteractable>().enabled = false;

            blockDown.position += Vector3.up;

            PlayAnimation(npcAnim, "NPCDrop", false);
            uiCheckpoint4[0].SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(3, () =>
            {
                uiCheckpoint4[0].SetActive(false);
                uiCheckpoint4[1].SetActive(true);
                uiCheckpoint4[2].SetActive(true);
            });
        }
        public void LookAtAccident()
        {
            Debug.Log("Get Look");
            if (!uiCheckpoint4[1].activeSelf) return;
            uiCheckpoint4[1].SetActive(false);
            uiCheckpoint4[2].SetActive(false);
            Checkpoint5Start();
        }
        #endregion
        #region Checkpoint 5
        private void Checkpoint5Start()
        {
            uiCheckpoint5[0].SetActive(true);
        }
        public void Checkpoint5Quiz(int num)
        {
            if (num == 0)
            {
                //GetScore
                scoreList[3] = true;

                uiCheckpoint5[2].SetActive(false);
                uiCheckpoint5[3].SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(2, () =>
                {
                    uiCheckpoint5[3].SetActive(false);
                    Checkpoint6Start();
                });
            }
            else
            {
                uiCheckpoint5[2].SetActive(false);
                uiCheckpoint5[4].SetActive(true);
                delay?.Kill();
                delay = DOVirtual.DelayedCall(2, () =>
                {
                    uiCheckpoint5[2].SetActive(true);
                    uiCheckpoint5[4].SetActive(false);
                });
            }
        }
        #endregion
        #region Checkpoint 6
        private void Checkpoint6Start()
        {
            PlayAnimation(npcAnim, "NPCDown", false);
            uiCheckpoint6[0].SetActive(true);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(5, () =>
            {
                uiCheckpoint6[0].SetActive(false);
                Checkpoint7Start();
            });
        }
        #endregion
        #region Checkpoint 7
        private void Checkpoint7Start()
        {
            uiCheckpoint7[0].SetActive(true);
        }
        public void Checkpoint7Quiz(int num)
        {
            uiCheckpoint7[0].SetActive(false);
            Checkpoint7EndGame();
        }
        private void Checkpoint7EndGame()
        {
            uiCheckpoint7[1].SetActive(true);
            uiCheckpoint1[5].SetActive(false);
            point1.gameObject.SetActive(false);
            delay?.Kill();
            delay = DOVirtual.DelayedCall(2, () =>
            {
                uiCheckpoint7[1].SetActive(false);
                ShowResult();
            });
        }
        private void Checkpoint7Continue()
        {

        }
        #endregion
    }
}
