using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCUIManager : MonoBehaviour
{
    public static PCUIManager Instance { get; private set; }

    public TMP_Text playerCountText;

    [Header("Category")]
    [SerializeField] private CCTVCategory currentCategory;
    public Category[] categories;
    public Button[] categoryButtons;
    List<Image> categoryImgs = new();
    public CCTVCategory CurrentCategory { get => currentCategory;
        set
        {
            currentCategory = value;
            UpdateCategoryButton();
            UpdateCategoryUI();
        }
    }

    [Header("Exit")]
    public Button exitBtn;
    public GameObject exitPopup;
    public Button exitConfirmBtn;
    public Button exitCancelBtn;

    private void Awake()
    {
        InitializeExit();
        InitializeCategory();
    }

    private void Update()
    {
        playerCountText.text = $"{CCTVController.Instance.viewers.Count}";
    }
    public void InitializeCategory()
    {
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            var btn = categoryButtons[index];
            categoryImgs.Add(btn.GetComponent<Image>());
            btn.onClick.AddListener(() =>
            {
                CurrentCategory = (CCTVCategory)index;
            });
        }

        UpdateCategoryButton();
        UpdateCategoryUI();
    }

    void UpdateCategoryUI()
    {
        foreach (var c in categories)
        {
            if (currentCategory == CCTVCategory.All)
            {
                c.gameObject.SetActive(true);
                c.SetHeaderActive(true);
            }
            else
            {
                c.gameObject.SetActive(c.CCTVCategory.Equals(currentCategory));
                c.SetHeaderActive(false);
            }
        }
    }

    void UpdateCategoryButton()
    {
        Color color = categoryImgs[(int)currentCategory].color;
        color.a = 0;

        foreach (var img in categoryImgs)
        {
            img.color = color;
        }

        color.a = 1;
        categoryImgs[(int)currentCategory].color = color;
    }

    public void InitializeExit()
    {
        exitBtn.onClick.AddListener(() =>
        {
            exitPopup.SetActive(true);
        });

        exitCancelBtn.onClick.AddListener(() =>
        {
            exitPopup.SetActive(false);
        });
        exitConfirmBtn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}

public enum CCTVCategory { All, Lobby, Scenario1, Scenario2, Scenario3, Scenario4, Scenario5 };