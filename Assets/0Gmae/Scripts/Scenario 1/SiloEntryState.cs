using Boy;
using UnityEngine;

public class SiloEntryState : State
{
    [Header("Setting")]
    public float delayChecngeState = 3;
    public float thresholdAngle = 75f;
    public float slopeLimit = 70;

    [Header("Reference")]
    public GameObject lid;
    public Transform ladder;
    public TriggerChecker climbChecker;
    public TriggerChecker floorChecker;

    [Header("HUD")]
    public GameObject lifelineHUD;
    public GameObject startMissionHUD;
    public GameObject riskyHUD;

    Player player;
    HUDState hUDState;
    bool isGrounded;
    bool isTrigger;
    public override void Awake()
    {
        base.Awake();
        player = Player.Instance;
        hUDState = GetComponent<HUDState>();

        climbChecker.OnEnter = () => {
            isGrounded = false;
            lid.SetActive(true);
            player?.StartClimbDownSilo();
            climbChecker.enabled = false;
        };

        floorChecker.OnEnter = () => {
            player?.StopClimbDownSilo();
            floorChecker.enabled = false;
            isGrounded = true;
        };

        lid.SetActive(true);
        foreach (var h in player.hooks)
        {
            h.checker.OnEnter += () => { isPass = true; };
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        lid.SetActive(false);
        climbChecker.enabled = true;
        floorChecker.enabled = true;

        isGrounded = false;
        isTrigger = false;

        player?.ShowHook();
        player?.SetSlopeLimit(slopeLimit);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (!isTrigger && isGrounded)
        {
            if (!IsFacingAwayFromLadder()) return;
            isTrigger = true;
            if (isPass)
            {
                hUDState.OpenHud(startMissionHUD);
            }
            else
            {
                hUDState.OpenHud(riskyHUD);
            }
            controller.NextState(delayChecngeState);
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        lid.SetActive(true);
        player?.HideHook();
        player?.ResetSlopeLimit();
    }
    bool IsFacingAwayFromLadder()
    {
        Vector3 headForward = Camera.main.transform.forward;
        Vector3 ladderForward = -ladder.forward;

        float angle = Vector3.Angle(headForward, ladderForward);

        return angle > thresholdAngle;
    }
}
