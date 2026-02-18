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
    RenderTexture renderTexture;

    public void Initialize(int index, Camera camera, Action<int> setIndex)
    {
        header.text = $"{prefix}{index + 1}";
        button = rawImage.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            setIndex?.Invoke(index);
        });

        if (camera.targetTexture != null)
        {
            rawImage.texture = camera.targetTexture;
            return;
        }

        renderTexture = new RenderTexture(1280, 720, 24);
        camera.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
    }

    private void OnDestroy()
    {
        if (renderTexture != null) Destroy(renderTexture);
    }
}
