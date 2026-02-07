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
    Scenario scenario;

    public override void Awake()
    {
        base.Awake();
        scenario = GetComponentInParent<Scenario>();
        hUDState = GetComponent<HUDState>();
        area.OnExit = () =>
        {
            controller.NextState();
        };
    }

    public override void StateEnter()
    {
        base.StateEnter();
        if (scenario) Player.Instance?.Teleport(teleportTarget, scenario.IsOwner);
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
