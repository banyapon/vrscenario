using Boy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PGroup
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private CheckpointController[] checkpointControllers;
        [SerializeField] private SummaryUI summaryUI;
        [SerializeField] private int scenarioIndex;
        [SerializeField] private TextMeshProUGUI scoreText;

        public Transform player;

        public List<bool> scoreList = new List<bool>();
        private int currentCheckpoint;
        Scenario scenario;
        private float timeUsed;
        private bool onPlaying;

        public static Action<int> OnCheckpointEnd = delegate { };
        public static Action OnRestartTrigger = delegate { };

        private void Awake()
        {
            scenario = GetComponentInParent<Scenario>();
            if (Player.Instance != null) player = Player.Instance.transform;
        }

        private void Start()
        {
            onPlaying = true;
            checkpointControllers[currentCheckpoint].StartStep();
            if (VRNetworkController.Instance != null && TrainingPlayerList.Instance != null && scenario != null)
            {
                if (VRNetworkController.Instance.inspector && TrainingPlayerList.Instance.selectedClientId == scenario.OwnerClientId)
                {
                    Player.Instance?.Teleport(Vector3.zero, Vector3.zero);
                }
            }
        }
        private void Update()
        {
            if (onPlaying)
                timeUsed += Time.deltaTime;
        }
        public void RestartCheckpoint()
        {
            onPlaying = true;
            OnRestartTrigger?.Invoke();
            timeUsed = 0;
            scoreList.Clear();
            currentCheckpoint = 0;
            summaryUI.gameObject.SetActive(false);
            for (int i = 0; i < checkpointControllers.Length; i++)
            {
                checkpointControllers[i].RestartStep();
            }
            checkpointControllers[currentCheckpoint].StartStep();
        }
        public void NextCheckpoint()
        {
            currentCheckpoint++;
            if (currentCheckpoint < checkpointControllers.Length)
            {
                //scoreList.Add(true);
                checkpointControllers[currentCheckpoint].StartStep();
            }
            else
            {
                //scoreList.Add(true);
                EndScenario();
            }
            OnCheckpointEnd?.Invoke(currentCheckpoint);
        }
        public void NextStep()
        {
            checkpointControllers[currentCheckpoint].NextStep();
        }
        private void EndScenario()
        {
            onPlaying = false;
            if (scenario) Player.Instance?.Teleport(Vector3.zero, Vector3.zero);
            summaryUI.gameObject.SetActive(true);

            Debug.Log(scoreList.Count);
            foreach (var item in scoreList)
            {
                Debug.Log($"{item}");
            }

            if (scenario.IsOwner)
            {
                summaryUI.ShowSummary(scoreList, SendApi);
                string listString = string.Join(",", scoreList.Select(b => b ? "1" : "0"));
                Debug.Log($"Not Inspector Shoot Score : {listString}");
                scenario.SentScoreToOther(listString);
            }
        }
        public void UpdateScoreUI(List<bool> scoreList)
        {
            summaryUI.ShowSummary(scoreList);
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

            string json = "";
            LoginController loginController = FindAnyObjectByType<LoginController>(FindObjectsInactive.Include);
            string role = loginController == null ? "" : loginController.GetPlayerRole();

            switch (scenarioIndex)
            {
                case 3:
                    var body3 = new
                    {
                        userEmail = APIManager.Instance.userEmail,
                        scenarioKey = "scenario3",
                        total_score = score,
                        stars = star,
                        details = new
                        {
                            assess_situation = scoreList[0],
                            ppe = scoreList[1],
                            action_incident = scoreList[2],
                            cleanse = scoreList[3],
                        },
                        time_used_seconds = (int)timeUsed,
                        remark = role
                    };
                    json = JsonConvert.SerializeObject(body3);
                    break;
                case 4:
                    var body4 = new
                    {
                        userEmail = APIManager.Instance.userEmail,
                        scenarioKey = "scenario4",
                        total_score = score,
                        stars = star,
                        details = new
                        {
                            explore_area = scoreList[0],
                            warning = scoreList[1],
                            action_emergency = scoreList[2]
                        },
                        time_used_seconds = (int)timeUsed,
                        remark = role
                    };
                    json = JsonConvert.SerializeObject(body4);
                    break;
            }

            print(json);

            APIManager.Instance.SaveSession<string>(json, (ok, msg, res) =>
            {
                print(msg);
                if (!ok) return;
                print(res);
            });
        }
        private void SendApi(int totalScore, float stars, List<string> details)
        {
            LoginController loginController = FindAnyObjectByType<LoginController>();
            string role = loginController == null ? "" : loginController.GetPlayerRole();

            if (string.IsNullOrEmpty(role))
            {
                role = PlayerPrefs.GetString("PlayerRole");
            }
            Debug.Log("Role : " + role);

            if (role == "Joiner Mode") return;

            string json = "";
            switch (scenarioIndex)
            {
                case 3:
                    var body3 = new
                    {
                        userEmail = APIManager.Instance.userEmail,
                        scenarioKey = "scenario3",
                        total_score = totalScore,
                        stars = stars,
                        details = new
                        {
                            assess_situation = details[0],
                            ppe = details[1],
                            action_incident = details[2],
                            cleanse = details[3],
                        },
                        time_used_seconds = (int)timeUsed,
                        remark = role
                    };
                    json = JsonConvert.SerializeObject(body3);
                    break;
                case 4:
                    var body4 = new
                    {
                        userEmail = APIManager.Instance.userEmail,
                        scenarioKey = "scenario4",
                        total_score = totalScore,
                        stars = stars,
                        details = new
                        {
                            explore_area = details[0],
                            warning = details[1],
                            action_emergency = details[2]
                        },
                        time_used_seconds = (int)timeUsed,
                        remark = role
                    };
                    json = JsonConvert.SerializeObject(body4);
                    break;
            }

            print(json);

            APIManager.Instance.SaveSession<string>(json, (ok, msg, res) =>
            {
                print(msg);
                if (!ok) return;
                print(res);
            });
        }
    }
}
