using Boy;
using DG.Tweening;
using Newtonsoft.Json;
using PGroup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioOneSummaryState : State
{
    [Space(10)]
    public float delayShowUI = 2;

    [Header("Reference")]
    public Timer timer;
    public Transform teleportTarget;
    public GameObject ambulance;
    public GameObject npc;
    public GameObject lid;

    [Header("UI")]
    public GameObject passHUD;
    public GameObject failHUD;
    public SummaryUI summaryUI;

    [Space(10)]
    public Button[] resetStateBtns;
    public Button[] backToLobbyBtns;

    [Space(10)]
    [SerializeField] private List<State> stateList = new();

    Player player = null;
    const float TIME_EPSILON = 0.001f;
    public override void Awake()
    {
        base.Awake();
        ambulance.SetActive(false);
        player = Player.Instance;

        foreach (var btn in resetStateBtns)
        {
            btn.onClick.AddListener(() => { controller.RestartScene(); });
        }

        foreach (var btn in backToLobbyBtns)
        {
            btn.onClick.AddListener(() => { controller.BackToLobby(); });
        }
    }

    public override void StateEnter()
    {
        base.StateEnter();
        ambulance.SetActive(true);
        npc.SetActive(false);
        lid.layer = 0;
        if (controller?.scenario)
        {
            player?.Teleport(teleportTarget);
            controller.scenario.StopCount();
        }

        summaryUI.gameObject.SetActive(false);
        passHUD.SetActive(false);
        failHUD.SetActive(false);

        timer.StopCount();
        bool hasTimeLeft = timer.timeLeft > TIME_EPSILON;
        passHUD.SetActive(hasTimeLeft);
        failHUD.SetActive(!hasTimeLeft);

        List<bool> resultList = new();
        foreach (var state in stateList)
        {
            resultList.Add(state.IsPass);
        }
        resultList.Add(hasTimeLeft);

        summaryUI?.ShowSummary(resultList, hasTimeLeft, SendApi);

        DOVirtual.DelayedCall(delayShowUI, () =>
        {
            summaryUI.gameObject.SetActive(true);
            passHUD.SetActive(false);
            failHUD.SetActive(false);
        });
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        npc.SetActive(true);
        ambulance.SetActive(false);
    }
    void SendApi(int totalScore, float stars, List<string> details)
    {
        if (controller == null) return;
        if (controller.scenario == null) return;
        if (!controller.scenario.IsOwner) return;

        float timeUsed = controller.scenario?.timeUsed ?? 0f;
        LoginController loginController = FindAnyObjectByType<LoginController>(FindObjectsInactive.Include);
        string role = loginController == null? "": loginController.GetPlayerRole();

        var body = new
        {
            userEmail = APIManager.Instance.userEmail,
            scenarioKey = "scenario1",
            total_score = totalScore,
            stars = stars,
            details = new
            {
                ppe = details[0],
                permit_to_work = details[1],
                silo_entry = details[2],
                gas_level = details[3],
                unconscious_found = details[4],
                rescue_operation = details[5],
                completed_within_4_minutes = details[6],
            },
            time_used_seconds = (int)timeUsed,
            remark = role
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
