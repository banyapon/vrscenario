using System;
using UnityEngine;
using UnityEngine.UI;
namespace Boy
{
    public class State : MonoBehaviour
    {
        [SerializeField] protected bool isPass;
        protected bool testFirstTime;
        public bool IsPass {
            get {

                return isPass && testFirstTime;
            }
            set => isPass = value;
        }

        [Header("State Setting")]
        public GameObject stateCanvas;
        public Button nextButton;
        public State nextState;
        protected StateController controller;
        protected StateSync sync;

        public Action onEnter, onUpdate, onExit;

        public virtual void Awake()
        {
            if (stateCanvas == null) stateCanvas = gameObject;
            stateCanvas.SetActive(false);
            controller = GetComponentInParent<StateController>();
            sync = GetComponent<StateSync>();
        }

        public virtual void StateEnter()
        {
            if (stateCanvas == null) stateCanvas = gameObject;
            gameObject.SetActive(true);
            stateCanvas.SetActive(true);

            isPass = false;
            testFirstTime = true;

            if (nextButton)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance.PlayButtonSFX();
                    if (nextState != null)
                    {
                        controller.GotoState(nextState);
                    }
                    else
                    {
                        controller.NextState();
                    }
                });
            }

            onEnter?.Invoke();
        }

        public virtual void StateUpdate()
        {
            onUpdate?.Invoke();
        }

        public virtual void StateExit()
        {
            stateCanvas.SetActive(false);
            gameObject.SetActive(false);
            NotifyValueChanged();
            onExit?.Invoke();
        }
        public void NotifyValueChanged()
        {
            if (controller != null && controller.IsOwner)
            {
                controller.OwnerReportStateValue(this, isPass, testFirstTime);
            }
        }

        public void ApplyValue(bool pass, bool first)
        {
            isPass = pass;
            testFirstTime = first;
        }
    }
}

public enum StateEventType { OnStateEnter, OnStateExit }