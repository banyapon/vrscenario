using Boy;
using DG.Tweening;
using UnityEngine;

public class VictimsToTopState : State
{
    [Space(20)]
    public GameObject victims2;
    public ScenarioOneHomeState homeStat;
    public AnimationClip pullUpClip;
    public GameObject reachedTopHUD;

    HUDState hUDState;

    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        victims2.SetActive(true);
        if (controller)
        {
            print($"CurrentState: {controller.GetCurrentState().name}");
            if (controller.GetCurrentState() == this)
            {
                homeStat.SetRopeAlpha(1);
            }
        }
        DOVirtual.DelayedCall(pullUpClip.length, () =>
        {
            victims2.SetActive(false);
            homeStat.SetRopeAlpha(0);
            hUDState.OpenHud(reachedTopHUD, () =>
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
        victims2.SetActive(false);
        homeStat.SetRopeAlpha(0);
    }
}
