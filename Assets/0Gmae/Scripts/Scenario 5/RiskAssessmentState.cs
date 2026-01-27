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

    public override void Awake()
    {
        base.Awake();

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
