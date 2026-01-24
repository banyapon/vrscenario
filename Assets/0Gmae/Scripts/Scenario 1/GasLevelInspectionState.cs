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

    [Header("HUD")]
    public GameObject emergencyHUD;
    public GameObject checkGasHUD;

    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        timer.StartCount();

        emergencyHUD.SetActive(true);
        checkGasHUD.SetActive(false);

        gasDetector.gameObject.SetActive(true);
        radio.gameObject.SetActive(true);

        DOVirtual.DelayedCall(hudDuration, () => {
            emergencyHUD.SetActive(false);
        }).SetLink(gameObject);

        gasDetector.OnEnter = () =>
        {
            isPass = true;
            controller.NextState();
            gasDetector.OnEnter = null;
        };

        radio.OnEnter = () =>
        {
            emergencyHUD.SetActive(false);
            testFirstTime = false;
            ShowHUD(checkGasHUD);
        };
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    Tween hudTween = null;
    void ShowHUD(GameObject hud)
    {
        hudTween?.Kill();
        HideHUD();
        hud.SetActive(true);
        hudTween = DOVirtual.DelayedCall(hudDuration, HideHUD).SetLink(gameObject);
    }
    void HideHUD()
    {
        checkGasHUD.SetActive(false);
    }
}
