using UnityEngine;
using Boy;
using DG.Tweening;

public class CheckGasState : State
{
    [Header("Setting")]
    public float duration = 5;

    [Header("Fake Value")]
    public float o2Fake;
    public float h2sFake;
    public float coFake;
    public float lelFake;

    [Header("Run Number Roller")]
    public RunNumberRoller o2;
    public RunNumberRoller h2s;
    public RunNumberRoller co;
    public RunNumberRoller lel;

    [Header("HUD")]
    public GameObject safeHUD;
    public GameObject notSafeHUD;

    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        print("Play NPC Animation here");
        hUDState?.HideHUD();

        o2.StartNumber(o2Fake, duration);
        h2s.StartNumber(h2sFake, duration);
        co.StartNumber(coFake, duration);
        lel.StartNumber(lelFake, duration);

        DOVirtual.DelayedCall(duration, () =>
        {
            hUDState.OpenHud(notSafeHUD, () =>
            {
                o2.StartNumberWithDuration(duration);
                h2s.StartNumberWithDuration(duration);
                co.StartNumberWithDuration(duration);
                lel.StartNumberWithDuration(duration);

                DOVirtual.DelayedCall(duration, () =>
                {
                    hUDState.OpenHud(safeHUD, () =>
                    {
                        controller.NextState();
                    });
                });
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
    }
}
