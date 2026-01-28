using Boy;
using UnityEngine;

public class SiloEntryState : State
{
    [Header("Setting")]
    public float delayChecngeState = 3;
    public float hudDuration = 2;
    public float thresholdAngle = 75f;

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
            isPass = true;
            isGrounded = true;
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
        isTrigger = false;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (!isTrigger && isGrounded)
        {
            if (!IsFacingAwayFromLadder()) return;
            isTrigger = true;
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
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        lid.SetActive(true);
    }
    bool IsFacingAwayFromLadder()
    {
        Vector3 headForward = Camera.main.transform.forward;
        Vector3 ladderForward = -ladder.forward;

        float angle = Vector3.Angle(headForward, ladderForward);
        print(angle);

        return angle > thresholdAngle;
    }
}
