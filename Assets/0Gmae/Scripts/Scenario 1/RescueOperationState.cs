using Boy;
using DG.Tweening;
using UnityEngine;

public class RescueOperationState : State
{
    [Header("Setting")]
    public float hudDuration = 2;

    [Header("Trigger Checker")]
    public TriggerChecker liftingSling;
    public TriggerChecker ordinaryRope;
    public TriggerChecker harness;

    [Header("HUD")]
    public GameObject reachedTopHUD;
    public GameObject notDesignedHUD;
    public GameObject liftingThingsHUD;

    Rigidbody liftingSlingRb;
    Rigidbody ordinaryRopeRb;
    Rigidbody harnessRb;

    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();

        liftingSlingRb = liftingSling.GetComponent<Rigidbody>();
        ordinaryRopeRb = ordinaryRope.GetComponent<Rigidbody>();
        harnessRb = harness.GetComponent<Rigidbody>();

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
            print("Play animation here");
            hUDState.OpenHud(reachedTopHUD);
            controller.NextState(hudDuration);
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();

        DOVirtual.DelayedCall(1, () =>
        {
            if (liftingSlingRb) liftingSlingRb.isKinematic = true;
            if (ordinaryRopeRb) ordinaryRopeRb.isKinematic = true;
            if (harnessRb) harnessRb.isKinematic = true;
        });

        liftingSling.gameObject.SetActive(true);
        ordinaryRope.gameObject.SetActive(true);
        harness.gameObject.SetActive(true);
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
