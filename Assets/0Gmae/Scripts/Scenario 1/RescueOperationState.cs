using Boy;
using DG.Tweening;
using UnityEngine;

public class RescueOperationState : State
{
    [Space(20)]
    public Victims victims;
    public GameObject victims2;
    public AnimationClip pullUpClip;

    [Header("Trigger Checker")]
    public TriggerChecker liftingSling;
    public TriggerChecker ordinaryRope;
    public TriggerChecker harness;

    [Header("HUD")]
    public GameObject reachedTopHUD;
    public GameObject notDesignedHUD;
    public GameObject liftingThingsHUD;

    HUDState hUDState;
    NetworkOwnershipContext context;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
        context = GetComponentInParent<NetworkOwnershipContext>();

        liftingSling.OnEnter += () => {
            hUDState.OpenHud(liftingThingsHUD);
            testFirstTime = false;
        };

        ordinaryRope.OnEnter += () => {
            hUDState.OpenHud(notDesignedHUD);
            testFirstTime = false;
        };

        harness.OnEnter += () => {
            isPass = true;
            harness.gameObject.SetActive(false);

            liftingSling.gameObject.SetActive(false);
            ordinaryRope.gameObject.SetActive(false);
            harness.gameObject.SetActive(false);

            //victims.Pullup(() => {
            //    victims.gameObject.SetActive(false);
            //    hUDState.OpenHud(reachedTopHUD, () =>
            //    {
            //        controller.NextState();
            //    });
            //});
            victims2.SetActive(true);
            victims.gameObject.SetActive(false);
            DOVirtual.DelayedCall(pullUpClip.length, () =>
            {
                victims2.SetActive(false);
                hUDState.OpenHud(reachedTopHUD, () =>
                {
                    controller.NextState();
                });
            });
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();

        liftingSling.gameObject.SetActive(true);
        ordinaryRope.gameObject.SetActive(true);
        harness.gameObject.SetActive(true);

        liftingSling.enabled = true;
        ordinaryRope.enabled = true;
        harness.enabled = true;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        victims.ResetAnimation();
        victims2.SetActive(false);
        victims.gameObject.SetActive(false);
    }
}
