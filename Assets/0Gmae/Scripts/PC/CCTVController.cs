using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCTVController : MonoBehaviour
{
    public static CCTVController Instance;

    public readonly Dictionary<ulong, Viewer> viewers = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterVRCamera(ulong clientId, List<Camera> vrCamera)
    {
        if (viewers.ContainsKey(clientId)) return;

        Viewer viewer = PCUIManager.Instance.InstantiateViewer();
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

        if (PCUIManager.Instance.activeViewer == viewer)
        {
            PCUIManager.Instance.ClearOtherCamera();
            //PCUIManager.Instance.CloseViewer();
        }

        Destroy(viewer.gameObject);
        viewers.Remove(clientId);
    }

    public void SetCameraIndex(ulong clientId, int index)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;
        viewer.Index = index;
    }

    public void SetViewerCategory(ulong clientId, CCTVCategory category)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;
        viewer.Category = category;
    }

    public void SetUserName(ulong clientId, string userName)
    {
        if (!viewers.TryGetValue(clientId, out var viewer)) return;
        viewer.userNameText.text = userName;
    }
}
