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

    QuizUI quizUI;
    public override void Awake()
    {
        base.Awake();
        quizUI = quizHud.GetComponent<QuizUI>();

        correctBtn.onClick.AddListener(() =>
        {
            isPass = true;
            controller.NextState(delayChangeState);
        });

        wrongBtn.onClick.AddListener(() =>
        {
            testFirstTime = false;
        });
    }

    public override void StateEnter()
    {
        base.StateEnter();
        explainHud.SetActive(true);
        quizHud.SetActive(false);
        quizUI.HideAllHuds();

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
}
