using System.Collections.Generic;
using UnityEngine;

namespace PGroup
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private CheckpointController[] checkpointControllers;

        private List<bool> scoreList = new List<bool>();
        private int currentCheckpoint;

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
                EndScenario();
            }
        }
        public void NextStep()
        {
            checkpointControllers[currentCheckpoint].NextStep();
        }
        private void EndScenario()
        {

        }

    }
}
