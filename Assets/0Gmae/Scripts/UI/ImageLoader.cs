using Boy;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public bool isMain = true;
    public string url;
    RawImage rawImage;

    private void OnEnable()
    {
        if (RoomCodeController.Instance == null) return;
        print(RoomCodeController.Instance.roomCode);
        if (rawImage == null) rawImage = GetComponent<RawImage>();
        if (rawImage == null) return;
        rawImage.enabled = false;
        //APIManager.Instance.DownloadImage(url, (texture) =>
        //{
        //    if (rawImage == null) return;
        //    rawImage.enabled = texture != null;
        //    if (texture == null) return;
        //    rawImage.texture = texture;
        //});
        if (isMain)
        {
            APIManager.Instance.GetMainLogo((texture) =>
            {
                if (rawImage == null) return;
                rawImage.enabled = texture != null;
                if (texture == null) return;
                rawImage.texture = texture;
            });
        }
        else
        {
            APIManager.Instance.GetSecondaryLogo((texture) =>
            {
                if (rawImage == null) return;
                rawImage.enabled = texture != null;
                if (texture == null) return;
                rawImage.texture = texture;
            });
        }
    }
}
