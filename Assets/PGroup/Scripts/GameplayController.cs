using Boy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PGroup
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private CheckpointController[] checkpointControllers;
        [SerializeField] private SummaryUI summaryUI;
        [SerializeField] private int scenarioIndex;

        public Transform player;

        public List<bool> scoreList = new List<bool>();
        private int currentCheckpoint;
        Scenario scenario;
        private float timeUsed;
        private bool onPlaying;

        public static Action<int> OnCheckpointEnd = delegate { };

        private void Awake()
        {
            scenario = GetComponentInParent<Scenario>();
            player = Camera.main.transform.parent.parent;
        }

        private void Start()
        {
            onPlaying = true;
            checkpointControllers[currentCheckpoint].StartStep();
        }
        private void Update()
        {
            if (onPlaying)
                timeUsed += Time.deltaTime;
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
            if (scenario) Player.Instance?.Teleport(Vector3.zero, Vector3.zero, scenario.IsOwner);
            summaryUI.gameObject.SetActive(true);
            summaryUI.ShowSummary(scoreList);
            SendScoreAPI();
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
                        remark = ""
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
                        remark = ""
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
