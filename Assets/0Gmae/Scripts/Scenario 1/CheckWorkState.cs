using UnityEngine;
using Boy;
public class CheckWorkState : State
{
    public TriggerChecker checker;
    public GameObject wall;
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
        wall.SetActive(false);
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
