using System;
using System.Collections.Generic;
using UnityEngine;

namespace PGroup
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private CheckpointController[] checkpointControllers;
        [SerializeField] private SummaryUI summaryUI;

        public Transform player;

        private List<bool> scoreList = new List<bool>();
        private int currentCheckpoint;
        Scenario scenario;

        public static Action<int> OnCheckpointEnd = delegate { };

        private void Awake()
        {
            scenario = GetComponentInParent<Scenario>();
            player = Camera.main.transform.parent.parent;
        }

        private void Start()
        {
            checkpointControllers[currentCheckpoint].StartStep();
        }
        public void NextCheckpoint()
        {
            currentCheckpoint++;
            if (currentCheckpoint < checkpointControllers.Length)
            {
                scoreList.Add(true);
                checkpointControllers[currentCheckpoint].StartStep();
            }
            else
            {
                scoreList.Add(true);
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
            if (scenario) Player.Instance?.Teleport(Vector3.zero, Vector3.zero, scenario.IsOwner);
            summaryUI.gameObject.SetActive(true);
            summaryUI.ShowSummary(scoreList);
        }

    }
}
