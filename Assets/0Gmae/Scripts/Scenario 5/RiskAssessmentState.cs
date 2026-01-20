using UnityEngine;
using UnityEngine.UI;
using Boy;
using DG.Tweening;

public class RiskAssessmentState : State
{
    [Header("Setting")]
    public float delayChangeState = 2;

    [Header("Reference")]
    public Button correctBtn;
    public Button wrongBtn;
    [Space(10)]
    public GameObject correctHud;
    public GameObject wrongHud;

    public override void Awake()
    {
        base.Awake();

        correctBtn.onClick.AddListener(() =>
        {
            SetEnableButton(false);
            correctHud.SetActive(true);
            isPass = true;

            controller.NextState(delayChangeState);
        });

        wrongBtn.onClick.AddListener(() =>
        {
            SetEnableButton(false);
            wrongHud.SetActive(true);
            testFirstTime = false;

            DOVirtual.DelayedCall(delayChangeState, CloseHud);
        });
    }

    public override void StateEnter()
    {
        base.StateEnter();
        CloseHud();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    void SetEnableButton(bool value)
    {
        correctBtn.interactable = value;
        wrongBtn.interactable = value;
    }

    void CloseHud()
    {
        correctHud.SetActive(false);
        wrongHud.SetActive(false);
        SetEnableButton(true);
    }
}
