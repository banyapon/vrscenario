using Boy;
using UnityEngine;
using UnityEngine.UI;

public class PermitToWorkState : State
{
    [Space(20)]
    public bool isCheckWorkPermit;
    public float delayChangState = 2;
    public Transform teleportTarget;
    public GameObject buttonGroup;

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
            buttonGroup.SetActive(false);
            hUDState.OpenHud(workPermitHUD, ShowButtonGroup);
        });

        gasDetectorBtn.onClick.AddListener(() =>
        {
            if (isCheckWorkPermit)
            {
                isPass = true;
                buttonGroup.SetActive(false);
                hUDState.OpenHud(gasHUD, ShowButtonGroup);
                gasDetectorBtn.interactable = false;
                controller.NextState(delayChangState);
                tankEntrance.enabled = false;
                print("Play NPC Animation here");
            }
            else
            {
                testFirstTime = false;
                buttonGroup.SetActive(false);
                hUDState.OpenHud(warningGasHUD, ShowButtonGroup);
            }
        });

        tankEntrance.OnEnter += () =>
        {
            testFirstTime = false;
            buttonGroup.SetActive(false);
            hUDState.OpenHud(tankHUD, ShowButtonGroup);
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

    void ShowButtonGroup()
    {
        buttonGroup.SetActive(true);
    }
}
