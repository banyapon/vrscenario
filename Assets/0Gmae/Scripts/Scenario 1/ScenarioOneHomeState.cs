using Boy;
using DG.Tweening;
using UnityEngine;

public class ScenarioOneHomeState : State
{
    [Header("Setting")]
    public float delayDuration = 2;

    [Header("Reference")]
    public GameObject popup1;
    public GameObject popup2;

    Tween delay;
    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();

        delay?.Kill();
        popup1.SetActive(true);
        popup2.SetActive(false);

        delay = DOVirtual.DelayedCall(delayDuration, () =>
        {
            popup1.SetActive(false);
            popup2.SetActive(true);
        })
            .OnComplete(() =>
        {
            delay = DOVirtual.DelayedCall(delayDuration, () =>
            {
                popup2.SetActive(false);
            })
            .OnComplete(() =>
            {
                controller.NextState();
            });
        });
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        delay?.Kill();
    }
}
