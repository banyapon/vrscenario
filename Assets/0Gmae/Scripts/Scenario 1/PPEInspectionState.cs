using Boy;
using System.Collections.Generic;
using UnityEngine;

public class PPEInspectionState : State
{
    [Space(20)]
    public float delayChangeState = 2;
    public PPESelector pPESelector;
    public override void Awake()
    {
        base.Awake();
        pPESelector.OnSelectionValidated += OnValidated;
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

    void OnValidated(bool isCorrect)
    {
        if (isCorrect)
        {
            isPass = true;
            controller.NextState(delayChangeState);
        }
        else
        {
            testFirstTime = false;
        }
    }
}
