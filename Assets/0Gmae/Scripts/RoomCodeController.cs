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

    [Header("Reference")]
    [SerializeField] private GameObject roomCodeUI;
    [SerializeField] private TMP_InputField roomCodeInputField;
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
        if (PlayerPrefs.HasKey("roomCode"))
        {
            CompleteRoomCodeFlow();
            roomCode = PlayerPrefs.GetString("roomCode");
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
            PlayerPrefs.SetString("roomCode", roomCode);
            CompleteRoomCodeFlow();
        });
    }

    void CompleteRoomCodeFlow()
    {
        hudState?.HideHUD();
        roomCodeUI.SetActive(false);
        nonNativeKeyboard?.Close();
        OnFinish?.Invoke();
        isFinish = true;
    }

    public void ShowKeyboard(TMP_InputField _input)
    {
        if (nonNativeKeyboard == null) return;
        NonNativeKeyboard.Instance.InputField = _input;
        NonNativeKeyboard.Instance.PresentKeyboard(_input.text);
    }
}