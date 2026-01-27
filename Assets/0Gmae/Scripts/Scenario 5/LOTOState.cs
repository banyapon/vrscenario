using UnityEngine;
using Boy;
using DG.Tweening;
using System.Collections.Generic;

public enum LOTOStep { StopButton, MainSwitchOff, LockoutTagout }
public class LOTOState : State
{
    [Header("Setting")]
    public float delayChangeState = 2;
    [SerializeField]
    private LOTOStep[] correctSequence =
    {
        LOTOStep.StopButton,
        LOTOStep.MainSwitchOff,
        LOTOStep.LockoutTagout
    };

    [Header("Reference")]
    public TriggerChecker stopBtn;
    public TriggerChecker mainSwitch;
    public TriggerChecker lockoutTagout;
    [Space(10)]
    public Transform mainSwitchTrans;
    public GameObject lockoutTagoutModel;
    public Transform lockoutTagoutGrab;
    [Space(10)]
    public GameObject explainHud;
    public GameObject correctHud;
    public GameObject wrongHud;

    ActivateStateEvent activateStateEvent;
    Vector3 lockoutTagoutPos;
    Vector3 lockoutTagoutRotate;
    private List<LOTOStep> pressedSequence = new();
    private HashSet<LOTOStep> pressedSteps = new();
    public override void Awake()
    {
        base.Awake();
        lockoutTagoutPos = lockoutTagoutGrab.position;
        lockoutTagoutRotate = lockoutTagoutGrab.eulerAngles;
        activateStateEvent = GetComponent<ActivateStateEvent>();
        //stopBtn.OnEnter += OnStopBtn;
        //mainSwitch.OnEnter += OnMainSwitch;
        //lockoutTagout.OnEnter += OnLockoutTagout;
    }

    public override void StateEnter()
    {
        base.StateEnter();

        stopBtn.OnEnter += OnStopBtn;
        mainSwitch.OnEnter += OnMainSwitch;
        lockoutTagout.OnEnter += OnLockoutTagout;

        ResetSequence();

        explainHud.SetActive(true);
        correctHud.SetActive(false);
        wrongHud.SetActive(false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();

        stopBtn.OnEnter -= OnStopBtn;
        mainSwitch.OnEnter -= OnMainSwitch;
        lockoutTagout.OnEnter -= OnLockoutTagout;
    }

    public void ResetSequence()
    {
        pressedSequence.Clear();
        pressedSteps.Clear();

        mainSwitch.enabled = true;
        stopBtn.enabled = true;
        lockoutTagout.enabled = true;

        explainHud.SetActive(true);
        correctHud.SetActive(false);
        wrongHud.SetActive(false);

        lockoutTagout.gameObject.SetActive(true);
        lockoutTagoutModel.SetActive(false);
        lockoutTagoutGrab.localPosition =  lockoutTagoutPos;
        lockoutTagoutGrab.localEulerAngles = lockoutTagoutRotate;
        lockoutTagoutGrab.gameObject.SetActive(true);

        if (activateStateEvent) activateStateEvent.SetTargetActive();
        RotateMainSwitch(Vector3.zero);
    }

    void RotateMainSwitch(Vector3 endValue)
    {
        float duration = 0.5f;
        Ease ease = Ease.Linear;
        DOTween.Kill(mainSwitchTrans);

        mainSwitchTrans.DOLocalRotate(endValue, duration)
            .SetEase(ease)
            .SetLink(mainSwitchTrans.gameObject);
    }
    public void PressStep(LOTOStep step)
    {
        if (pressedSteps.Contains(step))
            return;

        pressedSteps.Add(step);
        pressedSequence.Add(step);

        if (pressedSequence.Count == correctSequence.Length)
        {
            CheckResult();
        }
    }

    private void CheckResult()
    {
        bool isCorrect = true;

        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (pressedSequence[i] != correctSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        explainHud.SetActive(false);
        correctHud.SetActive(isCorrect);
        wrongHud.SetActive(!isCorrect);

        DOVirtual.DelayedCall(delayChangeState, () =>
        {
            if (isCorrect)
            {
                controller.NextState();
                isPass = true;
            }
            else
            {
                ResetSequence();
                testFirstTime = false;
            }
        });
    }
    void OnStopBtn()
    {
        stopBtn.enabled = false;
        PressStep(LOTOStep.StopButton);
    }

    void OnMainSwitch()
    {
        mainSwitch.enabled = false;
        RotateMainSwitch(Vector3.right * 80);
        PressStep(LOTOStep.MainSwitchOff);
    }

    void OnLockoutTagout()
    {
        lockoutTagout.enabled = false;
        lockoutTagout.gameObject.SetActive(false);
        lockoutTagoutModel.SetActive(true);
        lockoutTagoutGrab.gameObject.SetActive(false);
        PressStep(LOTOStep.LockoutTagout);
    }
}
