using UnityEngine;

namespace Boy
{
    public class Rung : MonoBehaviour
    {
        public GameObject highlightObj;
        TriggerChecker checker;

        private void Awake()
        {
            checker = GetComponent<TriggerChecker>();
            checker.OnEnter += () =>
            {
                SetHighlightObj(false);
            };
        }

        public void SetHighlightObj(bool value)
        {
            if (highlightObj == null) return;
            highlightObj.SetActive(value);
        }
    }
}
