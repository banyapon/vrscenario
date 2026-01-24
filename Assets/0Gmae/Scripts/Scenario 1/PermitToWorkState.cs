using Boy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PermitToWorkState : State
{
    [Space(20)]
    public bool isCheckWorkPermit;
    public float delayHideDuration = 2;
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
    public override void Awake()
    {
        base.Awake();
        player = Player.Instance;
        workPermitBtn.onClick.AddListener(() =>
        {
            workPermitBtn.interactable = false;
            isCheckWorkPermit = true;
            OpenHud(workPermitHUD);
        });

        gasDetectorBtn.onClick.AddListener(() =>
        {
            if (isCheckWorkPermit)
            {
                isPass = true;
                OpenHud(gasHUD);
                gasDetectorBtn.interactable = false;
                controller.NextState(delayChangState);
                print("Play NPC Animation here");
            }
            else
            {
                testFirstTime = false;
                OpenHud(warningGasHUD);
            }
        });

        tankEntrance.OnEnter += () =>
        {
            testFirstTime = false;
            OpenHud(tankHUD);
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();

        HideHud();
        delayHide?.Kill();
        player?.Teleport(teleportTarget);

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
        delayHide?.Kill();
    }

    Tween delayHide = null;
    public void OpenHud(GameObject HUD)
    {
        delayHide?.Kill();
        HideHud();
        HUD.SetActive(true);
        delayHide = DOVirtual.DelayedCall(delayHideDuration, HideHud);
    }

    public void HideHud()
    {
        gasHUD.SetActive(false);
        tankHUD.SetActive(false);
        warningGasHUD.SetActive(false);
        workPermitHUD.SetActive(false);
    }
}
