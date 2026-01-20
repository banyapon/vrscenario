using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCTVController : MonoBehaviour
{
    public static CCTVController Instance;

    [Header("UI")]
    [SerializeField] private Transform viewerRoot;
    [SerializeField] private Viewer viewerPrefab;
    [SerializeField] private RectTransform mainViewport;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject disconnectWarning;
    [SerializeField] private GameObject noPlayerText;

    [Header("Animation")]
    [SerializeField] private float tweenDuration = 0.35f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;

    private readonly Dictionary<ulong, Viewer> viewers = new();
    private Viewer activeViewer;
    private Transform canvasRoot;

    public bool IsOpen => activeViewer != null;

    private void Awake()
    {
        Instance = this;
        canvasRoot = GetComponentInParent<Canvas>().transform;
        closeButton.onClick.AddListener(CloseViewer);
        closeButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(() => { if (activeViewer) activeViewer.Index += 1; });
        backButton.onClick.AddListener(() => { if (activeViewer) activeViewer.Index -= 1; });
    }

    private void Update()
    {
        disconnectWarning.SetActive(activeViewer == null);
        noPlayerText.SetActive(viewerRoot.childCount == 0);
    }

    public void RegisterVRCamera(ulong clientId, List<Camera> vrCamera)
    {
        if (viewers.ContainsKey(clientId)) return;

        Viewer viewer = Instantiate(viewerPrefab, viewerRoot);
        viewer.Initialize(clientId, vrCamera);

        viewers.Add(clientId, viewer);
    }

    public void UpdateViewer(ulong clientId, List<Camera> vrCamera)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;
        viewer.UpdateCameraList(vrCamera);
    }

    public void UnregisterViewer(ulong clientId)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;

        if (activeViewer == viewer)
            CloseViewer();

        Destroy(viewer.gameObject);
        viewers.Remove(clientId);
    }

    public void SetCameraIndex(ulong clientId, int index)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;
        viewer.Index = index;
    }

    public void OpenViewer(Viewer viewer)
    {
        if (IsOpen) return;

        activeViewer = viewer;
        RectTransform viewRect = viewer.ViewportRect;

        viewRect.SetParent(canvasRoot);
        viewRect.DOSizeDelta(mainViewport.sizeDelta, tweenDuration).SetEase(tweenEase);
        viewRect.DOMove(mainViewport.position, tweenDuration).SetEase(tweenEase);

        closeButton.gameObject.SetActive(true);
    }

    public void CloseViewer()
    {
        if (!IsOpen) return;

        RectTransform viewRect = activeViewer.ViewportRect;

        viewRect.DOSizeDelta(activeViewer.RectTransform.sizeDelta, tweenDuration)
            .SetEase(tweenEase);

        viewRect.DOMove(activeViewer.transform.position, tweenDuration)
            .SetEase(tweenEase)
            .OnComplete(() =>
            {
                viewRect.SetParent(activeViewer.transform);
                activeViewer = null;
            });

        closeButton.gameObject.SetActive(false);
    }
}
