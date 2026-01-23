using Boy;
using System.Collections.Generic;
using UnityEngine;

public class GasLevelInspectionState : State
{
    [Space(20)]
    public Timer timer;

    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        timer.StartCount();
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
