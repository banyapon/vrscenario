using Boy;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public string url;
    RawImage rawImage;

    private void OnEnable()
    {
        if (rawImage == null) rawImage = GetComponent<RawImage>();
        if (string.IsNullOrEmpty(url) || rawImage == null) return;
        APIManager.Instance.DownloadImage(url, (texture) =>
        {
            rawImage.enabled = texture != null;
            if (texture == null) return;
            rawImage.texture = texture;
        });
    }
}
