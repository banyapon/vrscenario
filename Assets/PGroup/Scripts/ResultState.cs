using Boy;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ResultState : State
{
    public List<bool> scoreList;

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
            //player?.Teleport(teleportTarget, controller.scenario.IsOwner);
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
