using TMPro;
using UnityEngine;

public class Category : MonoBehaviour
{
    public CCTVCategory CCTVCategory = CCTVCategory.Lobby;
    public GameObject header;
    public Transform rootGrid;

    bool showHeader = true;
#if UNITY_EDITOR
    private void OnValidate()
    {
        name = $"Category {CCTVCategory.ToString()}";
        header.GetComponent<TMP_Text>().text = CCTVCategory.ToString();
    }
#endif

    private void Update()
    {
        header.SetActive(showHeader && rootGrid.childCount > 0);
    }

    public void SetHeaderActive(bool value)
    {
        showHeader = value;
        //header.SetActive(value);
    }
}
