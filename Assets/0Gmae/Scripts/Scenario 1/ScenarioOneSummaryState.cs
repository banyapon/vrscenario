using Boy;
using DG.Tweening;
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
        player?.Teleport(teleportTarget);

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

        summaryUI?.ShowSummary(resultList, hasTimeLeft);

        DOVirtual.DelayedCall(delayShowUI, () =>
        {
            summaryUI.gameObject.SetActive(true);
        });
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
