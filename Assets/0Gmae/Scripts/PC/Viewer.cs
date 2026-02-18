using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Viewer : MonoBehaviour
{
    [SerializeField] private CCTVCategory category = CCTVCategory.Lobby;
    [SerializeField] private int index = -1;

    [Header("Reference")]
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private Button expandBtn;
    [SerializeField] private RawImage viewportImage;
    public List<Camera> cameraList = new List<Camera>();

    public ulong ClientId { get; private set; }

    [HideInInspector] public RenderTexture renderTexture;

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

            if (cameraList[index].targetTexture != null)
            {
                renderTexture = cameraList[index].targetTexture;
            }
            else
            {
                if (renderTexture != null) Destroy(renderTexture);
                renderTexture = new RenderTexture(1280, 720, 24);
                cameraList[index].targetTexture = renderTexture;
            }

            viewportImage.texture = renderTexture;
        }
    }

    public CCTVCategory Category { get => category;
        set {
            category = value;
            PCUIManager.Instance.SetViewerParent(this);
            categoryNameText.text = category.ToString();
        }
    }

    public void Initialize(ulong clientId, List<Camera> _cameraList)
    {
        ClientId = clientId;
        cameraList = _cameraList;
        renderTexture = new RenderTexture(1920, 1080, 24);
        Index = 0;
        viewportImage.texture = renderTexture;

        expandBtn.onClick.AddListener(Expand);
    }

    public void UpdateCameraList(List<Camera> _cameraList)
    {
        cameraList = _cameraList;
        CheckCameraIndex();
        PCUIManager.Instance.UpdateOtherCamera();
    }

    public void CheckCameraIndex()
    {
        if (index >= cameraList.Count) Index = 0;
    }

    public void SetIndex(int index)
    {
        Index = index;
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

    public void Expand()
    {
        if (!PCUIManager.Instance.IsOpen)
        {
            PCUIManager.Instance.ExpandViewer(this);
        }
    }
    public void ClearNullInList()
    {
        cameraList.RemoveAll(item => item == null);
    }
}
