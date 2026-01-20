using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    public Image icon;

    [Header("Sprite")]
    public Sprite passIcon;
    public Sprite failIcon;

    public void SetIcon(bool isPass)
    {
        icon.sprite = isPass ? passIcon : failIcon;
    }
}
