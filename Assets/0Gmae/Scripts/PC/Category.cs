using TMPro;
using UnityEngine;

public class Category : MonoBehaviour
{
    public CCTVCategory CCTVCategory = CCTVCategory.Lobby;
    public GameObject header;


#if UNITY_EDITOR
    private void OnValidate()
    {
        name = $"Category {CCTVCategory.ToString()}";
        header.GetComponent<TMP_Text>().text = CCTVCategory.ToString();
    }
#endif

    public void SetHeaderActive(bool value)
    {
        header.SetActive(value);
    }
}
