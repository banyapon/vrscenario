using Boy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnconsciousFoundState : State
{
    [Header("Setting")]
    public float waitDuration = 3;
    public float hudDuration = 2;

    [Header("Button")]
    public Button safeBtn;
    public Button notSafeBtn;
    public TriggerChecker radio;

    [Header("HUD")]
    public GameObject radioReportHUD;
    public GameObject installHUD;
    public GameObject valueIsSafeHUD;

    [Space(10)]
    public RunNumberRoller[] runNumberRollers;
    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();

        safeBtn.onClick.AddListener(() => {
            hUDState.OpenHud(radioReportHUD);
            radio.gameObject.SetActive(true);
            radio.enabled = false;
            DOVirtual.DelayedCall(hudDuration, () => { radio.enabled = true; }).SetLink(gameObject);
        });

        notSafeBtn.onClick.AddListener(() => {
            hUDState.OpenHud(valueIsSafeHUD);
            testFirstTime = false;
        });
    }

    public override void StateEnter()
    {
        base.StateEnter();
        SetButtonInteractable(false);

        foreach (var r in runNumberRollers)
        {
            r.StartNumberWithDuration(waitDuration);
        }

        DOVirtual.DelayedCall(waitDuration, () =>
        {
            SetButtonInteractable(true);
        }).SetLink(gameObject);

        radio.OnEnter = () => {
            hUDState.OpenHud(installHUD);
            radio.enabled = false;
            isPass = true;
            controller.NextState(hudDuration);
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

    void SetButtonInteractable(bool value)
    {
        safeBtn.interactable = value;
        notSafeBtn.interactable = value;
    }
}
