using UnityEngine;
using Boy;

public class ScenarioFiveHomeState : State
{
    [Header("Setting")]
    public Transform teleportTarget;

    [Header("Reference")]
    public TriggerChecker emergencyStopBtn;
    public LOTOState lOTOState;

    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        //Player player = Player.Instance;
        //if (player && teleportTarget)
        //    player.Teleport(teleportTarget.localPosition, teleportTarget.localEulerAngles);

        emergencyStopBtn.OnEnter += OnEmergencyStop;
        lOTOState.ResetSequence();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        emergencyStopBtn.OnEnter -= OnEmergencyStop;
    }
    private void OnEmergencyStop()
    {
        controller.NextState();
    }
}
