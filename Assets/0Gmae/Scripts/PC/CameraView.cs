using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{
    [SerializeField] private string prefix = "Camera ";

    [Header("Reference")]
    [SerializeField] private TMP_Text header;
    [SerializeField] private RawImage rawImage;
    Button button;

    public void Initialize(int index)
    {
        header.text = $"{prefix}{index + 1}";
        button = rawImage.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            if (PCUIManager.Instance.activeViewer == null) return;
            PCUIManager.Instance.activeViewer.Index = index;
        });

        rawImage.texture = PCUIManager.Instance.activeViewer.GetOrCreateRenderTexture(index);
    }
}
