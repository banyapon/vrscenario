using UnityEngine;
using Boy;
using DG.Tweening;
using System.Collections.Generic;

public enum LOTOStep { StopButton, MainSwitchOff, LockoutTagout, LockoutTagout2 }
public class LOTOState : State
{
    [Header("Setting")]
    public float delayChangeState = 3;
    [SerializeField]
    private LOTOStep[] correctSequence =
    {
        LOTOStep.StopButton,
        LOTOStep.MainSwitchOff,
        LOTOStep.LockoutTagout,
        LOTOStep.LockoutTagout2
    };

    [Header("Reference")]
    public Transform mainSwitchTrans;
    [Space(10)]
    public TriggerChecker stopBtn;
    public TriggerChecker mainSwitch;
    public TriggerChecker lockoutTagout;
    public TriggerChecker lockoutTagout2;
    [Space(10)]
    public GameObject lockoutTagoutModel;
    public GameObject lockoutTagoutGrab;
    TransformResetter lockoutTagoutResetter;
    [Space(10)]
    public GameObject lockoutTagoutModel2;
    public GameObject lockoutTagoutGrab2;
    TransformResetter lockoutTagoutResetter2;
    [Space(10)]
    public GameObject explainHud;
    public GameObject correctHud;
    public GameObject wrongHud;

    ActivateStateEvent activateStateEvent;
    HUDState hUDState;
    private List<LOTOStep> pressedSequence = new();
    private HashSet<LOTOStep> pressedSteps = new();
    public override void Awake()
    {
        base.Awake();
        lockoutTagoutResetter = lockoutTagoutGrab.GetComponent<TransformResetter>();
        lockoutTagoutResetter2 = lockoutTagoutGrab2.GetComponent<TransformResetter>();
        activateStateEvent = GetComponent<ActivateStateEvent>();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();

        stopBtn.OnEnter += OnStopBtn;
        mainSwitch.OnEnter += OnMainSwitch;
        lockoutTagout.OnEnter += OnLockoutTagout;
        lockoutTagout2.OnEnter += OnLockoutTagout2;

        ResetSequence();

        hUDState.OpenHud(explainHud);
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
        lockoutTagout2.OnEnter -= OnLockoutTagout2;
    }

    public void ResetSequence()
    {
        pressedSequence.Clear();
        pressedSteps.Clear();

        mainSwitch.enabled = true;
        stopBtn.enabled = true;
        lockoutTagout.enabled = true;
        lockoutTagout2.enabled = true;

        //if (hUDState) hUDState.OpenHud(explainHud);

        if (lockoutTagoutResetter) lockoutTagoutResetter.ResetTransform();
        lockoutTagout.gameObject.SetActive(true);
        lockoutTagoutModel.SetActive(false);
        lockoutTagoutGrab.transform.SetParent(transform);
        lockoutTagoutGrab.SetActive(true);

        if (lockoutTagoutResetter2) lockoutTagoutResetter2.ResetTransform();
        lockoutTagout2.gameObject.SetActive(true);
        lockoutTagoutModel2.SetActive(false);
        lockoutTagoutGrab2.transform.SetParent(transform);
        lockoutTagoutGrab2.SetActive(true);

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

        hUDState.OpenHud(isCorrect? correctHud : wrongHud);

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
        RotateMainSwitch(Vector3.right * 180);
        PressStep(LOTOStep.MainSwitchOff);
    }

    void OnLockoutTagout()
    {
        lockoutTagout.enabled = false;
        lockoutTagout.gameObject.SetActive(false);
        lockoutTagoutModel.SetActive(true);
        //lockoutTagoutGrab.SetActive(false);
        PressStep(LOTOStep.LockoutTagout);
    }
    void OnLockoutTagout2()
    {
        lockoutTagout2.enabled = false;
        lockoutTagout2.gameObject.SetActive(false);
        lockoutTagoutModel2.SetActive(true);
        //lockoutTagoutGrab2.SetActive(false);
        PressStep(LOTOStep.LockoutTagout2);
    }
}
