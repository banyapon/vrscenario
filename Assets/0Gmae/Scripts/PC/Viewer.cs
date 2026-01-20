using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer : MonoBehaviour
{
    [SerializeField] private int index = -1;

    [Header("Reference")]
    [SerializeField] private RawImage viewportImage;
    [SerializeField] private List<Camera> cameraList = new List<Camera>();

    public ulong ClientId { get; private set; }
    public Vector2 OriginalSize { get; private set; }
    public RectTransform ViewportRect => viewportImage.rectTransform;
    public Button ViewportButton => viewportImage.GetComponent<Button>();

    private RenderTexture renderTexture;
    public RectTransform RectTransform => GetComponent<RectTransform>();

    public int Index { get => index;
        set {
            if (index == value) return;

            if (cameraList.Count > index && index >= 0)
            {
                if (cameraList[index] != null && cameraList[index].targetTexture != null)
                {
                    cameraList[index].targetTexture.Release();
                    cameraList[index].targetTexture = null;
                }
            }

            index = value;
            ClearNullInList();

            if (index >= cameraList.Count) index = 0;
            else if (index < 0) index = cameraList.Count - 1;

            if (renderTexture != null) Destroy(renderTexture);
            renderTexture = new RenderTexture(1920, 1080, 24);
            cameraList[index].targetTexture = renderTexture;
            viewportImage.texture = renderTexture;
        }
    }

    public void Initialize(ulong clientId, List<Camera> _cameraList)
    {
        ClientId = clientId;
        cameraList = _cameraList;
        renderTexture = new RenderTexture(1920, 1080, 24);
        Index = 0;
        viewportImage.texture = renderTexture;
        OriginalSize = ViewportRect.sizeDelta;

        ViewportButton.onClick.AddListener(OnClick);
    }

    public void UpdateCameraList(List<Camera> _cameraList)
    {
        cameraList = _cameraList;
        CheckCameraIndex();
    }

    public void CheckCameraIndex()
    {
        if (index >= cameraList.Count) Index = 0;
    }

    private void Update()
    {
        if (ViewportRect.parent == transform)
        {
            ViewportRect.sizeDelta = RectTransform.sizeDelta;
        }
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (viewportImage != null) {
            Destroy(viewportImage.gameObject);
        }
    }

    public void OnClick()
    {
        if (!CCTVController.Instance.IsOpen)
        {
            DOTween.Kill(viewportImage);
            CCTVController.Instance.OpenViewer(this);
        }
    }
    public void ClearNullInList()
    {
        cameraList.RemoveAll(item => item == null);
    }
}
