using Boy;
using UnityEngine;

public class GasLevelInspectionState : State
{
    public float hudDuration = 2;

    [Space(20)]
    public Timer timer;
    public XRGrabChecker gasDetector;
    public XRGrabChecker radio;
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
        radio.enabled = true;
        gasDetector.enabled = true;

        gasDetector.OnGrab = () =>
        {
            if (isPass) return;
            isPass = true;
            controller.NextState();
        };
        radio.OnGrab = () =>
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
