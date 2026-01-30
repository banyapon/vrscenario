using Boy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnconsciousFoundState : State
{
    [Header("Setting")]
    public float waitDuration = 3;

    [Header("Button")]
    public Button safeBtn;
    public Button notSafeBtn;
    public TriggerChecker radio;

    [Header("HUD")]
    public GameObject radioReportHUD;
    public GameObject installHUD;
    public GameObject valueIsSafeHUD;
    public GameObject quizUI;

    [Space(10)]
    public RunNumberRoller[] runNumberRollers;
    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();

        safeBtn.onClick.AddListener(() => {
            hUDState.OpenHud(radioReportHUD, () => { radio.enabled = true; });
            radio.gameObject.SetActive(true);
            radio.enabled = false;
            quizUI.gameObject.SetActive(false);
        });

        notSafeBtn.onClick.AddListener(() => {
            hUDState.OpenHud(valueIsSafeHUD, () => { quizUI.gameObject.SetActive(true); });
            testFirstTime = false;
            quizUI.gameObject.SetActive(false);
        });
    }

    public override void StateEnter()
    {
        base.StateEnter();
        SetButtonInteractable(false);
        quizUI.gameObject.SetActive(true);

        foreach (var r in runNumberRollers)
        {
            r.ResetDisplay();
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
            controller.NextState(hUDState.hudDuration);
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
