using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PCUIManager : MonoBehaviour
{
    public static PCUIManager Instance { get; private set; }

    public bool isMute = true;
    [SerializeField] private TMP_Text playerCountText;
    public GameObject noPlayerText;
    public GameObject loading;

    [Header("Viewer")]
    public Viewer activeViewer;
    [SerializeField] private Viewer viewerPrefab;
    [SerializeField] private GameObject viewerUI;
    [SerializeField] private CameraView cameraViewPrefab;
    [Space(20)]
    [SerializeField] private RawImage mainViewPort;
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Transform otherCameraParent;
    [SerializeField] private Button backBtn;
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private GameObject disconnectText;
    [SerializeField] private Button muteBtn;


    public bool IsOpen => activeViewer != null;

    [Header("Category")]
    [SerializeField] private CCTVCategory currentCategory;
    public RectTransform contentCategory;
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

    [Header("Restart")]
    public Button restartBtn;
    public GameObject restartPopup;
    public Button restartConfirmBtn;
    public Button restartCancelBtn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeExit();
        InitializeRestart();
        InitializeCategory();
        InitializeViewer();

        muteBtn.onClick.AddListener(() =>
        {
            isMute = !isMute;
            muteBtn.transform.GetChild(0).gameObject.SetActive(!isMute);
            muteBtn.transform.GetChild(1).gameObject.SetActive(isMute);
            activeViewer?.SetAudio(isMute);
        });

        Rebuild();
    }

    private void Update()
    {
        if (CCTVController.Instance != null && CCTVController.Instance.viewers != null)
        {
            playerCountText.text = $"{CCTVController.Instance.viewers.Count}";
        }
        disconnectText.SetActive(!IsOpen);
        SetActiveNoPlayerText();
        ViewerUpdate();
    }

    void SetActiveNoPlayerText()
    {
        if (currentCategory.Equals(CCTVCategory.All))
        {
            noPlayerText.SetActive(CCTVController.Instance.viewers.Count == 0);
            return;
        }

        Category category = null;
        switch (currentCategory)
        {
            case CCTVCategory.Lobby:
                category = FindCategory(CCTVCategory.Lobby);
                break;
            case CCTVCategory.Scenario1:
                category = FindCategory(CCTVCategory.Scenario1);
                break;
            case CCTVCategory.Scenario2:
                category = FindCategory(CCTVCategory.Scenario2);
                break;
            case CCTVCategory.Scenario3:
                category = FindCategory(CCTVCategory.Scenario3);
                break;
            case CCTVCategory.Scenario4:
                category = FindCategory(CCTVCategory.Scenario4);
                break;
            case CCTVCategory.Scenario5:
                category = FindCategory(CCTVCategory.Scenario5);
                break;
        }

        noPlayerText.SetActive(category.rootGrid.childCount == 0);
    }

    #region Initialize
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
    public void InitializeRestart()
    {
        restartBtn.onClick.AddListener(() =>
        {
            restartPopup.SetActive(true);
        });

        restartCancelBtn.onClick.AddListener(() =>
        {
            restartPopup.SetActive(false);
        });
        restartConfirmBtn.onClick.AddListener(() =>
        {
            Destroy(PCNetworkBootstrap.Instance);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    #endregion

    #region Category

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
                c.gameObject.SetActive(currentCategory.Equals(c.CCTVCategory));
                c.SetHeaderActive(false);
            }
        }

        //Rebuild();
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

    Category FindCategory(CCTVCategory category)
    {
        foreach (var c in categories)
        {
            if (c.CCTVCategory.Equals(category))
            {
                return c;
            }
        }

        return null;
    }

    #endregion

    #region Viewer

    void InitializeViewer()
    {
        viewerUI.SetActive(false);
        backBtn.onClick.AddListener(CloseViewer);
    }

    void ViewerUpdate()
    {
        if (activeViewer == null) return;
        categoryNameText.text = activeViewer.Category.ToString();
        mainViewPort.texture = activeViewer.GetOrCreateRenderTexture(activeViewer.Index);
    }

    public Viewer InstantiateViewer()
    {
        Category category = FindCategory(CCTVCategory.Lobby);
        Viewer viewer = Instantiate(viewerPrefab, category.rootGrid);
        return viewer;
    }

    public void SetViewerParent(Viewer viewer)
    {
        Category category = FindCategory(viewer.Category);
        viewer.transform.SetParent(category.rootGrid);
        //Rebuild();
    }

    public void ExpandViewer(Viewer viewer)
    {
        if (IsOpen || viewerUI.activeInHierarchy) return;
        activeViewer = viewer;
        activeViewer.SetAudio(isMute);
        UpdateOtherCamera();
        userNameText.text = viewer.userNameText.text;
        viewerUI.SetActive(true);
    }

    public void CloseViewer()
    {
        if (!IsOpen && !viewerUI.activeInHierarchy) return;
        activeViewer.SetAudio(true);
        activeViewer = null;
        ClearOtherCamera();
        viewerUI.SetActive(false);
        //Rebuild();
    }

    public void UpdateOtherCamera()
    {
        if (!IsOpen) return;
        ClearOtherCamera();
        for (int i = 0; i < activeViewer.cameraList.Count; i++)
        {
            CameraView cameraView = Instantiate(cameraViewPrefab, otherCameraParent);
            cameraView.Initialize(i);
        }
    }

    public void ClearOtherCamera()
    {
        for (int i = otherCameraParent.childCount - 1; i >= 0; i--)
        {
            Destroy(otherCameraParent.GetChild(i).gameObject);
        }
        activeViewer?.ClearAllExcept();
    }

    #endregion

    #region Utility

    public void Rebuild(RectTransform target = null)
    {
        if (target == null) target = contentCategory;
        StartCoroutine(_Rebuild(target));
    }

    IEnumerator _Rebuild(RectTransform target)
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(target);

        ContentSizeFitter contentSizeFitter = target.GetComponent<ContentSizeFitter>();
        float delayDuration = 0.25f;

        yield return new WaitForSeconds(delayDuration);
        contentSizeFitter.enabled = false;

        yield return new WaitForSeconds(delayDuration);
        contentSizeFitter.enabled = true;
        
        Rebuild();
    }

    #endregion
}

public enum CCTVCategory { All, Lobby, Scenario1, Scenario2, Scenario3, Scenario4, Scenario5 };