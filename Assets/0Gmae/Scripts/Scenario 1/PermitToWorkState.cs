using Boy;
using UnityEngine;
using UnityEngine.UI;

public class PermitToWorkState : State
{
    [Space(20)]
    public bool isCheckWorkPermit;
    public float delayChangState = 2;
    public Transform teleportTarget;

    [Header("Work Permit Case")]
    public Button workPermitBtn;
    public GameObject workPermitHUD;

    [Header("Gas Detector Case")]
    public Button gasDetectorBtn;
    public GameObject gasHUD;
    public GameObject warningGasHUD;

    [Header("Walk down to the tank Case")]
    public TriggerChecker tankEntrance;
    public GameObject tankHUD;

    Player player = null;
    HUDState hUDState;
    public override void Awake()
    {
        base.Awake();
        player = Player.Instance;
        hUDState = GetComponent<HUDState>();
        workPermitBtn.onClick.AddListener(() =>
        {
            workPermitBtn.interactable = false;
            isCheckWorkPermit = true;
            hUDState.OpenHud(workPermitHUD);
        });

        gasDetectorBtn.onClick.AddListener(() =>
        {
            if (isCheckWorkPermit)
            {
                isPass = true;
                hUDState.OpenHud(gasHUD);
                gasDetectorBtn.interactable = false;
                controller.NextState(delayChangState);
                tankEntrance.enabled = false;
                print("Play NPC Animation here");
            }
            else
            {
                testFirstTime = false;
                hUDState.OpenHud(warningGasHUD);
            }
        });

        tankEntrance.OnEnter += () =>
        {
            testFirstTime = false;
            hUDState.OpenHud(tankHUD);
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();
        player?.Teleport(teleportTarget);

        tankEntrance.enabled = true;
        isCheckWorkPermit = false;
        workPermitBtn.interactable = true;
        gasDetectorBtn.interactable = true;
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
