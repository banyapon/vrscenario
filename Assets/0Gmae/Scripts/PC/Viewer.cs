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
    private Dictionary<int, RenderTexture> renderTextureDict = new Dictionary<int, RenderTexture>();

    public ulong ClientId { get; private set; }
    public int Index
    {
        get => index;
        set
        {
            if (index == value) return;

            index = value;
            ClearNullInList();

            if (index >= cameraList.Count) index = 0;
            else if (index < 0) index = cameraList.Count - 1;

            viewportImage.texture = GetOrCreateRenderTexture(index);
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
        Index = 0;
        expandBtn.onClick.AddListener(Expand);
    }

    public void UpdateCameraList(List<Camera> _cameraList)
    {
        ClearAllRenderTextures();
        cameraList = _cameraList;
        int oldIndex = index;
        index = -1;
        if (oldIndex >= cameraList.Count) Index = 0;
        else Index = oldIndex;
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
        ClearAllRenderTextures();
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
        if (cameraList == null) return;
        cameraList.RemoveAll(item => item == null);
    }
    public RenderTexture GetOrCreateRenderTexture(int camIndex)
    {
        ClearNullInList();
        if (cameraList == null || cameraList.Count == 0)
            return null;

        if (camIndex >= cameraList.Count)
            camIndex = 0;
        else if (camIndex < 0)
            camIndex = cameraList.Count - 1;

        Camera cam = cameraList[camIndex];

        if (cam == null)
            return null;

        if (renderTextureDict.TryGetValue(camIndex, out RenderTexture existingRT))
        {
            cam.enabled = true;
            cam.targetTexture = existingRT;
            return existingRT;
        }

        RenderTexture newRT = new RenderTexture(1280, 720, 24);
        newRT.Create();

        cam.targetTexture = newRT;
        cam.enabled = true;

        renderTextureDict.Add(camIndex, newRT);

        return newRT;
    }
    public void ClearAllRenderTextures()
    {
        foreach (var pair in renderTextureDict)
        {
            int camIndex = pair.Key;
            RenderTexture rt = pair.Value;

            if (cameraList.Count > camIndex && cameraList[camIndex] != null)
            {
                cameraList[camIndex].enabled = false;
                cameraList[camIndex].targetTexture = null;
            }

            if (rt != null)
            {
                rt.Release();
                Destroy(rt);
            }
        }

        renderTextureDict.Clear();
    }
    public void ClearAllExcept()
    {
        List<int> keys = new List<int>(renderTextureDict.Keys);

        foreach (int key in keys)
        {
            if (key == index)
                continue;

            if (cameraList.Count > key && cameraList[key] != null)
            {
                cameraList[key].enabled = false;
                cameraList[key].targetTexture = null;
            }

            if (renderTextureDict[key] != null)
            {
                renderTextureDict[key].Release();
                Destroy(renderTextureDict[key]);
            }

            renderTextureDict.Remove(key);
        }
    }

}
