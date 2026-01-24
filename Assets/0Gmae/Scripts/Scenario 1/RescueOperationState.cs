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

    ResetToDefault liftingSlingResetter;
    ResetToDefault ordinaryRopeResetter;
    ResetToDefault harnessResetter;

    public override void Awake()
    {
        base.Awake();

        liftingSlingResetter = liftingSling.GetComponent<ResetToDefault>();
        ordinaryRopeResetter = ordinaryRope.GetComponent<ResetToDefault>();
        harnessResetter = harness.GetComponent<ResetToDefault>();

        liftingSling.OnEnter += () => {
            ShowHUD(liftingThingsHUD);
            testFirstTime = false;
        };

        ordinaryRope.OnEnter += () => {
            ShowHUD(notDesignedHUD);
            testFirstTime = false;
        };

        harness.OnEnter += () => {
            isPass = true;
            print("Play animation here");
            ShowHUD(reachedTopHUD);
            controller.NextState(hudDuration);
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();
        HideHUD();

        liftingSlingResetter.ResetTransform();
        ordinaryRopeResetter.ResetTransform();
        harnessResetter.ResetTransform();

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
        reachedTopHUD.SetActive(false);
        notDesignedHUD.SetActive(false);
        liftingThingsHUD.SetActive(false);
    }
}
