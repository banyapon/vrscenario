using UnityEngine;

namespace PGroup
{
    public class TriggerController : MonoBehaviour
    {
        [SerializeField] private GameplayController gameplayController;
        private GameObject[] triggers;
        private int currentTrigger;
        private void Start()
        {
            SetupTriggers();
        }
        private void SetupTriggers()
        {
            triggers = new GameObject[transform.childCount];

            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i] = transform.GetChild(i).gameObject;
                triggers[i].SetActive(false);
            }
            StartTrigger();
        }
        private void StartTrigger()
        {
            triggers[currentTrigger].SetActive(true);
        }
        public void GetTrigger(GameObject obj)
        {
            if (obj == triggers[currentTrigger])
            {
                currentTrigger++;
                if (currentTrigger < triggers.Length)
                {
                    obj.SetActive(false);
                    triggers[currentTrigger].SetActive(true);
                }
                else
                {
                    gameplayController.NextStep();
                }
            }
        }
    }
}
