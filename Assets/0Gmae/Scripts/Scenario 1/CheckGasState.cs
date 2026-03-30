using UnityEngine;
using Boy;
using DG.Tweening;

public class CheckGasState : State
{
    [Header("Setting")]
    public float duration = 5;
    public NPC npc;
    public Transform npcSpot1;
    public Transform npcSpot2;

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

    string parameterName = "check"; //"Blend Pick", "check"
    HUDState hUDState;
    Tween delay;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        npc.SetFloat(parameterName, 0);
        ChangeNpcPose();
        hUDState?.HideHUD();

        o2.StartNumber(o2Fake, duration);
        h2s.StartNumber(h2sFake, duration);
        co.StartNumber(coFake, duration);
        lel.StartNumber(lelFake, duration);

        delay?.Kill();
        delay = DOVirtual.DelayedCall(duration, () =>
        {
            ChangeNpcPose(0);
            hUDState.OpenHud(notSafeHUD, () =>
            {
                ChangeNpcPose();
                o2.StartNumberWithDuration(duration);
                h2s.StartNumberWithDuration(duration);
                co.StartNumberWithDuration(duration);
                lel.StartNumberWithDuration(duration);

                delay = DOVirtual.DelayedCall(duration, () =>
                {
                    ChangeNpcPose(0);
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
        poseTween?.Kill();
        delay?.Kill();
        npc.SetFloat(parameterName, 0);
        ChangeNpcPose(0);
    }

    Tween poseTween;
    void ChangeNpcPose(float value = 0.5f)
    {
        float duration = 2f;

        float currentValue = npc.GetFloat(parameterName);

        poseTween?.Kill();
        poseTween = DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;
                npc.SetFloat(parameterName, x);
            },
            value,
            duration
        );
        npc.SetBool(parameterName, value > 0.45f);

        DOTween.Kill(npc.transform);
        Transform spot = value > 0.45f ? npcSpot2 : npcSpot1;
        //duration = value > 0.45f ? 1.75f : 2f;
        duration = 1f;
        Ease ease = value > 0.45f ? Ease.Linear : Ease.Linear;
        npc.transform.DORotate(spot.eulerAngles, duration)
            .SetLink(gameObject).SetEase(ease);
        npc.transform.DOMove(spot.position, duration)
            .SetLink(gameObject).SetEase(ease);
    }
}
