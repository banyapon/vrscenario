using UnityEngine;
using UnityEngine.UI;

public class ScenarioButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    public GameObject selectBg;
    public GameObject unSelectBg;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetSelect(false);
    }

    public void SetSelect(bool isSelect)
    {
        selectBg.SetActive(isSelect);
        unSelectBg.SetActive(!isSelect);
    }
}
