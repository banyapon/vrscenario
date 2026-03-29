using UnityEngine;
using Boy;

public class ScenarioFiveHomeState : State
{
    [Header("Setting")]
    public Transform teleportTarget;

    [Header("Reference")]
    public TriggerChecker area;
    public GameObject explainHUD;
    public LOTOState lOTOState;
    public MachineDoor machineDoor;
    public NPC npc;
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
        npc.SetBool("pick", true);
        npc.SetBool("walk", false);
        npc.SetBool("check out", false);
        npcAnimator.SetBool("move", false);
        npc.SetForceRotation(false);
        npc.SetForcePosition(false);
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
