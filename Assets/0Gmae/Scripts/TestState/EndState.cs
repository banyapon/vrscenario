using UnityEngine;
using UnityEngine.UI;
using Boy;

public class EndState : State
{
    [Space(20)]
    public Button restartBtn;
    public Button backToLobbyBtn;

    public override void Awake()
    {
        base.Awake();
        restartBtn.onClick.AddListener(controller.RestartScene);
        backToLobbyBtn.onClick.AddListener(controller.BackToLobby);
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