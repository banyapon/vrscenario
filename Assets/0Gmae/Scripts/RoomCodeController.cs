using Boy;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(HUDState))]
public class RoomCodeController : MonoBehaviour
{
    public static RoomCodeController Instance;
    public bool isFinish;
    public string roomCode;

    [Header("Setting")]
    [SerializeField] private bool deleteCode;
    [SerializeField] private TextMeshProUGUI roomCodeText;

    [Header("Reference")]
    [SerializeField] private GameObject roomCodeUI;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject enterCodeWarningHUD;


    [Header("Event")]
    public UnityEvent OnStart;
    public UnityEvent OnFinish;

    HUDState hudState;
    NonNativeKeyboard nonNativeKeyboard;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isFinish = false;
#if !UNITY_EDITOR
        deleteCode = false;
#endif
        if (deleteCode) PlayerPrefs.DeleteKey("roomCode");
        roomCodeUI.SetActive(false);
    }

    private void Start()
    {
        resetButton.onClick.AddListener(ResetCode);

        if (PlayerPrefs.HasKey("roomCode"))
        {
            CompleteRoomCodeFlow();
            roomCode = PlayerPrefs.GetString("roomCode");
            SetCodeText();
            print($"Room code is {roomCode}");
            return;
        }

        Initialize();
    }

    void Initialize()
    {
        OnStart?.Invoke();

        hudState = GetComponent<HUDState>();
        hudState.HideHUD();

        nonNativeKeyboard = NonNativeKeyboard.Instance;

        roomCodeUI.SetActive(true);
        roomCodeInputField.onSelect.AddListener(x => ShowKeyboard(roomCodeInputField));

        confirmButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(roomCodeInputField.text))
            {
                print("Please enter the room code.");
                hudState.OpenHud(enterCodeWarningHUD);
                return;
            }

            roomCode = roomCodeInputField.text;
            SetCodeText();
            PlayerPrefs.SetString("roomCode", roomCode);
            CompleteRoomCodeFlow();
        });
    }

    void CompleteRoomCodeFlow()
    {
        roomCodeText.text = "CODE : " + roomCode;
        hudState?.HideHUD();
        roomCodeUI.SetActive(false);
        nonNativeKeyboard?.Close();
        OnFinish?.Invoke();
        isFinish = true;
    }

    public void ResetCode()
    {
        PlayerPrefs.DeleteKey("roomCode");
        roomCode = "";
        SetCodeText();
        if (PCNetworkBootstrap.Instance != null)
        {
            PCNetworkBootstrap.Instance.StopHost();
        }
        Initialize();
    }

    void SetCodeText()
    {
        codeText.text = $"CODE : {roomCode}".ToUpper();
    }

    public void ShowKeyboard(TMP_InputField _input)
    {
        if (nonNativeKeyboard == null) return;
        NonNativeKeyboard.Instance.InputField = _input;
        NonNativeKeyboard.Instance.PresentKeyboard(_input.text);
    }
    public void ResetRoomCode()
    {
        roomCodeText.text = "CODE : -";
        PlayerPrefs.DeleteKey("roomCode");
        Initialize();
    }
}