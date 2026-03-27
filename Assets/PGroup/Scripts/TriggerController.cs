using DG.Tweening;
using UnityEngine;

namespace PGroup
{
    public class TriggerController : MonoBehaviour
    {
        [SerializeField] private GameplayController gameplayController;
        [SerializeField] private bool isNextStep;
        [SerializeField] private GameObject done;

        private GameObject[] triggers;
        private int currentTrigger;
        private Tween delay = null;


        private void OnEnable()
        {
            SetupTriggers();
        }
        private void SetupTriggers()
        {
            currentTrigger = 0;
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
                    if (isNextStep)
                    {
                        obj.SetActive(false);
                        gameplayController.NextStep();
                    }
                    else
                    {
                        obj.SetActive(false);
                        if (done != null)
                        {
                            done.SetActive(true);
                            delay?.Kill();
                            delay = DOVirtual.DelayedCall(3, () =>
                            {
                                done.SetActive(false);
                            });
                        }
                    }
                }
            }
        }
    }
}
