using UnityEngine;
using Boy;
using DG.Tweening;

public class SecurityVerificationState : State
{
    [Header("Setting")]
    public float delayChangeState = 2;

    [Header("Reference")]
    public GameObject machineSound;
    [Space(10)]
    public TriggerChecker startBtn;
    public TriggerChecker door;
    [Space(10)]
    public GameObject correctHud;
    public GameObject wrongHud;
    [Space(10)]
    public MachineDoor machineDoor;

    public override void Awake()
    {
        base.Awake();
        startBtn.OnEnter += OnStartBtn;
        door.OnEnter += OnDoorOpen;
    }

    public override void StateEnter()
    {
        base.StateEnter();
        CloseHud();
        machineSound.SetActive(false);
        door.enabled = true;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        machineDoor.Close();
        SetEnableHandTrigger(false);
        door.enabled = false;
    }
    void SetEnableHandTrigger(bool value)
    {
        startBtn.enabled = value;
        door.enabled = value;
    }

    void CloseHud()
    {
        correctHud.SetActive(false);
        wrongHud.SetActive(false);
        SetEnableHandTrigger(true);
    }

    void OnStartBtn()
    {
        SetEnableHandTrigger(false);
        correctHud.SetActive(true);
        //machineSound.SetActive(true);
        isPass = true;
        controller.NextState(delayChangeState);
    }

    void OnDoorOpen()
    {
        SetEnableHandTrigger(false);
        wrongHud.SetActive(true);
        testFirstTime = false;
        DOVirtual.DelayedCall(delayChangeState, CloseHud);
    }
}
