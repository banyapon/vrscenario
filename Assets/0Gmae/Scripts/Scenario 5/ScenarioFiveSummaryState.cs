using Boy;
using Newtonsoft.Json;
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

        if (controller?.scenario)
        {
            Player.Instance?.Teleport(teleportTarget, controller.scenario.IsOwner);
            controller.scenario.StopCount();
        }

        List<bool> resultList = new();
        foreach (var state in stateList)
        {
            resultList.Add(state.IsPass);
        }

        summaryUI?.ShowSummary(resultList, SendApi);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    void SendApi(int totalScore, float stars, List<string> details)
    {
        if (controller == null) return;
        if (controller.scenario == null) return;
        if (!controller.scenario.IsOwner) return;
        //if (controller.scenario.IsHost) return;

        float timeUsed = controller.scenario?.timeUsed ?? 0f;

        var body = new
        {
            userEmail = APIManager.Instance.userEmail,
            scenarioKey = "scenario5",
            total_score = totalScore,
            stars = stars,
            details = new
            {
                risk_assessment = details[0],
                loto = details[1],
                security_verification = details[2],
                harm_prevention = details[3],
            },
            time_used_seconds = (int)timeUsed,
            remark = ""
        };

        string json = JsonConvert.SerializeObject(body);
        print(json);

        APIManager.Instance.SaveSession<string>(json, (ok, msg, res) =>
        {
            print(msg);
            if (!ok) return;
            print(res);
        });
    }
}
