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
        //if (!VRNetworkController.Instance.inspector) return;
        Debug.Log(VRNetworkController.Instance.playerId);
        Debug.Log(controller?.scenario.OwnerClientId);
        Debug.Log(controller?.scenario.NetworkObjectId);
        Debug.Log(controller?.scenario.NetworkBehaviourId);
        Player player = Player.Instance;
        if (controller?.scenario)
        {
            player?.Teleport(teleportTarget, controller.scenario.IsOwner);
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
