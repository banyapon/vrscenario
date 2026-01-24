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
    public override void Awake()
    {
        base.Awake();

        safeBtn.onClick.AddListener(() => {
            ShowHUD(radioReportHUD);
            radio.gameObject.SetActive(true);
            radio.enabled = false;
            DOVirtual.DelayedCall(hudDuration, () => { radio.enabled = true; }).SetLink(gameObject);
        });

        notSafeBtn.onClick.AddListener(() => {
            ShowHUD(valueIsSafeHUD);
            testFirstTime = false;
        });
    }

    public override void StateEnter()
    {
        base.StateEnter();

        HideHUD();
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
            ShowHUD(installHUD);
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
        installHUD.SetActive(false);
        radioReportHUD.SetActive(false);
        valueIsSafeHUD.SetActive(false);
    }
}
