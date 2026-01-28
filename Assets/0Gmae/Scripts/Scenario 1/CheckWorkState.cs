using UnityEngine;
using Boy;
public class CheckWorkState : State
{
    public TriggerChecker checker;
    public override void Awake()
    {
        base.Awake();
        checker.OnEnter = () =>
        {
            controller.NextState();
        };
    }
    public override void StateEnter()
    {
        base.StateEnter();
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
