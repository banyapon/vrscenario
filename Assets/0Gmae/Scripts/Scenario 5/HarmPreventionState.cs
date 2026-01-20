using UnityEngine;
using Boy;
using UnityEngine.UI;
using DG.Tweening;

public class HarmPreventionState : State
{
    [Header("Setting")]
    public float waitNpcDuration = 3;
    public float delayChangeState = 2;

    [Header("Reference")]
    public GameObject explainHud;
    public GameObject quizHud;
    [Space(10)]
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
        explainHud.SetActive(true);
        quizHud.SetActive(false);

        DOVirtual.DelayedCall(waitNpcDuration, () =>
        {
            explainHud.SetActive(false);
            quizHud.SetActive(true);
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
