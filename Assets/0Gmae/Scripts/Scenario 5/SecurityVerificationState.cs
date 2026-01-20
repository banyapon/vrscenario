using UnityEngine;
using Boy;
using DG.Tweening;

public class SecurityVerificationState : State
{
    [Header("Setting")]
    public float delayChangeState = 2;

    [Header("Reference")]
    public TriggerChecker startBtn;
    public TriggerChecker cover;
    public Transform coverTrans;
    [Space(10)]
    public GameObject correctHud;
    public GameObject wrongHud;

    public override void Awake()
    {
        base.Awake();
        startBtn.OnEnter += OnStartBtn;
        cover.OnEnter += OnCover;
    }

    public override void StateEnter()
    {
        base.StateEnter();
        CloseHud();
        //startBtn.OnHandEnter += OnStartBtn;
        //cover.OnHandEnter += OnCover;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        //startBtn.OnHandEnter -= OnStartBtn;
        //cover.OnHandEnter -= OnCover;

        SetEnableHandTrigger(false);
    }
    void SetEnableHandTrigger(bool value)
    {
        startBtn.enabled = value;
        cover.enabled = value;
    }

    void CloseHud()
    {
        correctHud.SetActive(false);
        wrongHud.SetActive(false);
        RotateCover(Vector3.zero);
        SetEnableHandTrigger(true);
    }
    void RotateCover(Vector3 endValue)
    {
        float duration = 0.5f;
        Ease ease = Ease.Linear;
        DOTween.Kill(coverTrans);

        coverTrans.DOLocalRotate(endValue, duration)
            .SetEase(ease)
            .SetLink(coverTrans.gameObject);
    }

    void OnStartBtn()
    {
        SetEnableHandTrigger(false);
        correctHud.SetActive(true);
        isPass = true;
        controller.NextState(delayChangeState);
    }

    void OnCover()
    {
        SetEnableHandTrigger(false);
        wrongHud.SetActive(true);
        RotateCover(Vector3.right * 80);
        testFirstTime = false;
        DOVirtual.DelayedCall(delayChangeState, CloseHud);
    }
}
