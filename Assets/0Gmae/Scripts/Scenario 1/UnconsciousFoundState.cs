using Boy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnconsciousFoundState : State
{
    [Header("Setting")]
    public float waitDuration = 3;
    public GameObject gasDetector;

    [Header("Button")]
    public Button safeBtn;
    public Button notSafeBtn;
    public GrabChecker radio;

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
            hUDState.OpenHud(radioReportHUD);
            quizUI.gameObject.SetActive(false);
            radio.enabled = true;
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

        radio.OnGrab = () => {
            hUDState.OpenHud(installHUD);
            radio.enabled = false;
            isPass = true;
            controller.NextState(hUDState.hudDuration);
        };
        radio.enabled = false;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        gasDetector.SetActive(false);
        radio.gameObject.SetActive(false);
    }

    void SetButtonInteractable(bool value)
    {
        safeBtn.interactable = value;
        notSafeBtn.interactable = value;
    }
}
