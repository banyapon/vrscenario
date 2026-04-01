using Boy;
using DG.Tweening;
using UnityEngine;

public class VictimsToTopState : State
{
    [Space(20)]
    public GameObject victims2;
    public Material ropeMaterial;
    public AnimationClip pullUpClip;
    public GameObject reachedTopHUD;

    Color originalColor;
    HUDState hUDState;

    public override void Awake()
    {
        base.Awake();
        originalColor = ropeMaterial.color;
        hUDState = GetComponent<HUDState>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        victims2.SetActive(true);
        originalColor.a = 1;
        ropeMaterial.color = originalColor;
        DOVirtual.DelayedCall(pullUpClip.length, () =>
        {
            victims2.SetActive(false);
            originalColor.a = 0;
            ropeMaterial.color = originalColor;
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
        originalColor.a = 0;
        ropeMaterial.color = originalColor;
    }
}
