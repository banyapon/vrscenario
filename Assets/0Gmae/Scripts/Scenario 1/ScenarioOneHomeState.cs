using Boy;
using DG.Tweening;
using UnityEngine;

public class ScenarioOneHomeState : State
{
    [Header("Setting")]
    public float delayDuration = 2;
    public Transform teleportTarget;

    [Header("Reference")]
    public GameObject popup1;
    public GameObject popup2;
    [Space(10)]
    public Victims victims;
    public GameObject radio;
    public GameObject gasDetector;
    [Space(10)]
    public GameObject liftingSling;
    public GameObject ordinaryRope;
    public GameObject harness;

    Tween delay;
    ResetToDefault radioResetter;
    ResetToDefault gasDetectorResetter;
    public override void Awake()
    {
        base.Awake();
        radioResetter = radio.GetComponent<ResetToDefault>();
        gasDetectorResetter = gasDetector.GetComponent<ResetToDefault>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        Player player = Player.Instance;
        if (player && teleportTarget)
            player.Teleport(teleportTarget.localPosition, teleportTarget.localEulerAngles);
        
        ResetScenario();

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

    void ResetScenario()
    {
        victims.gameObject.SetActive(true);
        victims.ResetAnimation();

        radio.SetActive(false);
        gasDetector.SetActive(false);

        liftingSling.SetActive(false);
        ordinaryRope.SetActive(false);
        harness.SetActive(false);

        radioResetter?.ResetTransform();
        gasDetectorResetter?.ResetTransform();
    }
}
