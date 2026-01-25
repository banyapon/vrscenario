using Boy;
using DG.Tweening;
using UnityEngine;

public class SiloEntryState : State
{
    [Header("Setting")]
    public float delayChecngeState = 3;
    public float hudDuration = 2;

    [Header("Reference")]
    public GameObject lid;
    public TriggerChecker climbChecker;
    public TriggerChecker floorChecker;

    [Header("HUD")]
    public GameObject lifelineHUD;
    public GameObject startMissionHUD;
    public GameObject riskyHUD;

    Player player;
    public override void Awake()
    {
        base.Awake();
        player = Player.Instance;

        climbChecker.OnEnter = () => {
            lid.SetActive(true);
            player?.StartClimbDownSilo();
            climbChecker.enabled = false;
        };

        floorChecker.OnEnter = () => {
            player?.StopClimbDownSilo();
            floorChecker.enabled = false;
            isPass = true;
            ShowHUD(startMissionHUD);
            print("Play animetion here");
            controller.NextState(delayChecngeState);
        };

        lid.SetActive(true);
    }

    public override void StateEnter()
    {
        base.StateEnter();
        lid.SetActive(false);
        climbChecker.enabled = true;
        floorChecker.enabled = true;

        testFirstTime = false;
        HideHUD();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        lid.SetActive(true);
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
        lifelineHUD.SetActive(false);
        startMissionHUD.SetActive(false);
        riskyHUD.SetActive(false);
    }
}
