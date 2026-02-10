using UnityEngine;
using Boy;
using NUnit.Framework.Interfaces;

public class ScenarioFiveHomeState : State
{
    [Header("Setting")]
    public Transform teleportTarget;

    [Header("Reference")]
    public TriggerChecker area;
    public GameObject explainHUD;
    public LOTOState lOTOState;
    public MachineDoor machineDoor;
    public Animator npcAnimator;

    HUDState hUDState;

    public override void Awake()
    {
        base.Awake();
        hUDState = GetComponent<HUDState>();
        area.OnExit = () =>
        {
            controller.NextState();
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();
        if (controller?.scenario)
        {
            Player.Instance?.Teleport(teleportTarget, controller.scenario.IsOwner);
            controller.scenario.RestartCount();
        }
        hUDState?.OpenHud(explainHUD);
        lOTOState.ResetSequence();
        machineDoor.Close();
        npcAnimator.SetBool("move", false);
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
