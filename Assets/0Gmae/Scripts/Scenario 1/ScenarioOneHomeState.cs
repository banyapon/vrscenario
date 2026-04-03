using Boy;
using DG.Tweening;
using Obi;
using UnityEngine;

public class ScenarioOneHomeState : State
{
    [Header("Setting")]
    public float delayDuration = 2;
    public Transform teleportTarget;

    [Header("Reference")]
    public GameObject popup1;
    public GameObject popup2;
    [Space(10)]
    public HookController hook;
    public Victims victims;
    public GameObject victims2;
    public Material ropeMaterial;
    public ObiRopeExtrudedRenderer ropeExtrudedRenderer;
    public GameObject radio;
    public GameObject gasDetector;
    public GameObject wall;
    [Space(10)]
    public GameObject liftingSling;
    public GameObject ordinaryRope;
    public GameObject harness;

    [HideInInspector] public Material ropeMatInstance;
    Tween delay;
    ResetToDefault radioResetter;
    ResetToDefault gasDetectorResetter;
    public override void Awake()
    {
        base.Awake();
        //ropeMatInstance = new Material(ropeMaterial);
        //ropeExtrudedRenderer.material = ropeMatInstance;
        radioResetter = radio.GetComponent<ResetToDefault>();
        gasDetectorResetter = gasDetector.GetComponent<ResetToDefault>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        Player player = Player.Instance;
        if (controller?.scenario)
        {
            player?.Teleport(teleportTarget);
            controller.scenario.RestartCount();
        }

        ResetScenario();

        delay?.Kill();
        popup1.SetActive(true);
        popup2.SetActive(false);

        delay = DOVirtual.DelayedCall(delayDuration, () =>
        {
            popup1.SetActive(false);
            popup2.SetActive(true);
        })
            .OnComplete(() =>
        {
            delay = DOVirtual.DelayedCall(delayDuration, () =>
            {
                popup2.SetActive(false);
            })
            .OnComplete(() =>
            {
                controller.NextState();
            });
        });
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        delay?.Kill();
    }

    void ResetScenario()
    {
        hook.Hide();
        wall.SetActive(true);

        victims.gameObject.SetActive(true);
        victims.ResetAnimation();

        victims2.SetActive(false);

        radio.SetActive(false);
        gasDetector.SetActive(false);

        liftingSling.SetActive(false);
        ordinaryRope.SetActive(false);
        harness.SetActive(false);

        radioResetter?.ResetTransform();
        gasDetectorResetter?.ResetTransform();

        SetRopeAlpha(0);
    }

    public void SetRopeAlpha(float alpha)
    {
        print($"SetRopeAlpha: {ropeMaterial}");
        bool codition = false;
        PlayerData[] playerDatas = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        if (controller != null)
        {
            if (controller.IsOwner)
            {
                codition = true;
            }
            else if (controller.IsHost)
            {
                foreach (var playerData in playerDatas)
                {
                    if (playerData == null) continue;
                    if (controller.OwnerClientId == playerData.OwnerClientId)
                    {
                        codition = true;
                        break;
                    }
                }
            }
        }

        foreach (var playerData in playerDatas)
        {
            if (playerData == null) continue;
            if (playerData.IsInspector && playerData.IsOwner)
            {
                codition = true;
                break;
            }
        }
        if (codition)
        {
            print($"alpha: {alpha}");
            if (ropeMaterial == null) return;
            Color color = ropeMaterial.color;
            color.a = alpha;
            ropeMaterial.color = color;
            print($"color: {ropeMaterial.color}");
        }

        //print($"SetRopeAlpha: {ropeMatInstance}");
        //print($"alpha: {alpha}");
        //if (ropeMatInstance == null) return;
        //Color color = ropeMatInstance.color;
        //color.a = alpha;
        //ropeMatInstance.color = color;
        //print($"color: {ropeMatInstance.color}");
    }

    private void OnDestroy()
    {
        if (ropeMatInstance != null) Destroy(ropeMatInstance);
    }
}
