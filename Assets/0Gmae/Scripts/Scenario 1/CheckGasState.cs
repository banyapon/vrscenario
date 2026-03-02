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

    string parameterName = "Blend Pick";
    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        npc.SetFloat(parameterName, 0);
        ChangeNpcPose(1);
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
                        ChangeNpcPose(0);
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

    Tween poseTween;
    void ChangeNpcPose(float value)
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

        DOTween.Kill(npc.transform);
        Transform spot = value > 0.95f ? npcSpot2 : npcSpot1;
        Ease ease = value > 0.95f ? Ease.OutQuart : Ease.InExpo;
        npc.transform.DORotate(spot.eulerAngles, duration)
            .SetLink(gameObject).SetEase(ease);
        npc.transform.DOMove(spot.position, duration)
            .SetLink(gameObject).SetEase(ease);
    }
}
