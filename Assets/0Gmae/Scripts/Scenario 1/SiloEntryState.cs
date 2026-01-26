using Boy;
using DG.Tweening;
using NUnit.Framework.Interfaces;
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
    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        player = Player.Instance;
        hUDState = GetComponent<HUDState>();

        climbChecker.OnEnter = () => {
            lid.SetActive(true);
            player?.StartClimbDownSilo();
            climbChecker.enabled = false;
        };

        floorChecker.OnEnter = () => {
            player?.StopClimbDownSilo();
            floorChecker.enabled = false;
            isPass = true;
            if (testFirstTime)
            {
                hUDState.OpenHud(startMissionHUD);
            }
            else
            {
                hUDState.OpenHud(riskyHUD);
            }
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
}
