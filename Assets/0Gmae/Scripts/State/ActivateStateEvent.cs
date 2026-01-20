
using Boy;
using UnityEngine;

public class ActivateStateEvent : MonoBehaviour
{
    public bool value;
    public StateEventType stateEvent;
    public GameObject[] targets;

    State State;

    private void Awake()
    {
        if (targets == null) return;

        State = GetComponent<State>();
        if (State == null) return;

        switch (stateEvent)
        {
            case StateEventType.OnStateEnter:
                State.onEnter += SetTargetActive;
                break;
            case StateEventType.OnStateExit:
                State.onExit += SetTargetActive;
                break;
        }
    }

    public void SetTargetActive()
    {
        foreach (var t in targets)  t.SetActive(value);
    }

    private void OnDestroy()
    {
        if (State == null) return;

        switch (stateEvent)
        {
            case StateEventType.OnStateEnter:
                State.onEnter -= SetTargetActive;
                break;
            case StateEventType.OnStateExit:
                State.onExit -= SetTargetActive;
                break;
        }
    }
}