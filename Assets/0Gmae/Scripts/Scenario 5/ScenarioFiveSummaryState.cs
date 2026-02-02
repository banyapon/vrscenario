using Boy;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioFiveSummaryState : State
{
    [Header("Setting")]
    public Transform teleportTarget;

    [Header("Reference")]
    public SummaryUI summaryUI;
    [SerializeField] private List<State> stateList = new();

    public override void Awake()
    {
        base.Awake();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        Player.Instance?.Teleport(teleportTarget);

        List<bool> resultList = new();
        foreach (var state in stateList)
        {
            resultList.Add(state.IsPass);
        }

        summaryUI?.ShowSummary(resultList);
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
