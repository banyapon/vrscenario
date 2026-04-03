using Boy;
using DG.Tweening;
using UnityEngine;

public class TeleportState : State
{
    public Transform teleportTarget;

    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        Player player = Player.Instance;
        if (controller?.scenario)
        {
            player?.Teleport(teleportTarget);
        }
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
