using Boy;
using DG.Tweening;
using UnityEngine;

public class GasLevelInspectionState : State
{
    public float hudDuration = 2;

    [Space(20)]
    public Timer timer;
    public TriggerChecker gasDetector;
    public TriggerChecker radio;
    public Victims victims;

    [Header("HUD")]
    public GameObject emergencyHUD;
    public GameObject checkGasHUD;
    HUDState hUDState;

    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();

        gasDetector.OnEnter = () =>
        {
            if (isPass) return;
            isPass = true;
            controller.NextState();
        };

        radio.OnEnter = () =>
        {
            emergencyHUD.SetActive(false);
            testFirstTime = false;
            hUDState.OpenHud(checkGasHUD);
        };

        victims.Unconscious(() =>
        {
            timer.ReStart();
            gasDetector.gameObject.SetActive(true);
            radio.gameObject.SetActive(true);
            hUDState.OpenHud(emergencyHUD);
        });
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }
}
