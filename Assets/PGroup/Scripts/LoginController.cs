using Boy;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System;
using System.Collections;
using System.Text;
using TMPro;
using Unity.Services.Relay.Models;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PGroup
{
    public class LoginController : MonoBehaviour
    {
        #region Input UI
        [Header("Panel")]
        [HideInInspector][SerializeField] private GameObject loginPanel;
        [HideInInspector][SerializeField] private GameObject registerPanel;
        [HideInInspector][SerializeField] private GameObject resetPasswordPanel;
        [HideInInspector][SerializeField] private GameObject sentEmailResetPasswordPanel;
        [HideInInspector][SerializeField] private GameObject searchingPanel;
        [HideInInspector][SerializeField] private GameObject notFoundPanel;
        [HideInInspector][SerializeField] private GameObject regisSuccessPanel;
        [HideInInspector][SerializeField] private GameObject resetPasswordSuccessPanel;
        [HideInInspector][SerializeField] private GameObject notCorrectPasswordPanel;

        [Header("Button")]
        [HideInInspector][SerializeField] private Button standaloneButton;
        [HideInInspector][SerializeField] private Button playerButton;
        [HideInInspector][SerializeField] private Button inspectorButton;
        [HideInInspector][SerializeField] private Button forgotPasswordButton;
        [HideInInspector][SerializeField] private Button sendEmailPasswordButton;
        [HideInInspector][SerializeField] private Button loginButton;
        [HideInInspector][SerializeField] private Button signUpButton;
        [HideInInspector][SerializeField] private Button retryButton;
        [HideInInspector][SerializeField] private Button cancelButton;
        [HideInInspector][SerializeField] private Button registerButton;
        [HideInInspector][SerializeField] private Button backToLoginFromRegisButton;
        [HideInInspector][SerializeField] private Button resetPasswordButton;
        [HideInInspector][SerializeField] private Button backToLoginFromResetButton;
        [HideInInspector][SerializeField] private Button backToLoginFromSentEmailTokenButton;
        [HideInInspector][SerializeField] private Button closeRegisSuccessButton;
        [HideInInspector][SerializeField] private Button closeResetPasswordSuccessButton;
        [HideInInspector][SerializeField] private Button closeNotCorrectPasswordButton;
            
        [Header("InputField")]
        [HideInInspector][SerializeField] private TMP_InputField usernameInputField;
        [HideInInspector][SerializeField] private TMP_InputField passwordInputField;
        [HideInInspector][SerializeField] private TMP_InputField firstnameInputField;
        [HideInInspector][SerializeField] private TMP_InputField lastnameInputField;
        [HideInInspector][SerializeField] private TMP_InputField emailInputField;
        [HideInInspector][SerializeField] private TMP_InputField passwordRegisInputField;
        [HideInInspector][SerializeField] private TMP_InputField confirmPasswordInputField;
        [HideInInspector][SerializeField] private TMP_InputField emailSentResetInputField;
        [HideInInspector][SerializeField] private TMP_InputField emailResetInputField;
        [HideInInspector][SerializeField] private TMP_InputField tokenResetInputField;
        [HideInInspector][SerializeField] private TMP_InputField newPasswordResetInputField;

        [Header("Toggle")]
        [HideInInspector][SerializeField] private Toggle policyToggle;

        [Header("Scripts")]
        [SerializeField] private VRNetworkController networkController;
        [SerializeField] private NonNativeKeyboard nonNativeKeyboard;
        #endregion
        #region Public Fuction Button
        private void Awake()
        {
            standaloneButton.onClick.AddListener(() => ButtonStandalone());
            playerButton.onClick.AddListener(() => ButtonPlayer());
            inspectorButton.onClick.AddListener(() => ButtonInpector());
            forgotPasswordButton.onClick.AddListener(() => ButtonForgotPassword());
            loginButton.onClick.AddListener(() => ButtonLogin());
            signUpButton.onClick.AddListener(() => ButtonSignup());
            retryButton.onClick.AddListener(() => ButtonRetry());
            cancelButton.onClick.AddListener(() => ButtonCancel());
            registerButton.onClick.AddListener(() => ButtonRegister());
            backToLoginFromRegisButton.onClick.AddListener(() => ButtonBacktoLogin());
            resetPasswordButton.onClick.AddListener(() => ButtonResetPassword());
            backToLoginFromResetButton.onClick.AddListener(() => ButtonBacktoLogin());
            sendEmailPasswordButton.onClick.AddListener(() => ButtonSentEmailPassword());
            backToLoginFromSentEmailTokenButton.onClick.AddListener(() => ButtonBacktoLogin());
            closeNotCorrectPasswordButton.onClick.AddListener(() => ButtonCloseNotCorrectPassword());

            usernameInputField.onSelect.AddListener(x => ShowKeyboard(usernameInputField));
            passwordInputField.onSelect.AddListener(x => ShowKeyboard(passwordInputField));
            firstnameInputField.onSelect.AddListener(x => ShowKeyboard(firstnameInputField));
            lastnameInputField.onSelect.AddListener(x => ShowKeyboard(lastnameInputField));
            emailInputField.onSelect.AddListener(x => ShowKeyboard(emailInputField));
            passwordRegisInputField.onSelect.AddListener(x => ShowKeyboard(passwordRegisInputField));
            confirmPasswordInputField.onSelect.AddListener(x => ShowKeyboard(confirmPasswordInputField));
            emailSentResetInputField.onSelect.AddListener(x => ShowKeyboard(emailSentResetInputField));
            emailResetInputField.onSelect.AddListener(x => ShowKeyboard(emailResetInputField));
            tokenResetInputField.onSelect.AddListener(x => ShowKeyboard(tokenResetInputField));
            newPasswordResetInputField.onSelect.AddListener(x => ShowKeyboard(newPasswordResetInputField));
        }
        public void ButtonStandalone() { OnStandAlone(); }
        public void ButtonPlayer() { OnPlayer(); }
        public void ButtonInpector() { OnInspector(); }
        public void ButtonLogin() { OnLogin(); }
        public void ButtonForgotPassword() { OnForgotPassword(); }
        public void ButtonSignup() { OnSignUp(); }
        public void ButtonCancel() { OnCancel(); }
        public void ButtonRetry() { OnRetry(); }
        public void ButtonRegister() { OnRegister(); }
        public void ButtonBacktoLogin() { OnBacktoLogin(); }
        public void ButtonResetPassword() { OnResetPassword(); }
        public void ButtonSentEmailPassword() { OnSentEmailResetPassword(); }
        public void ShowKeyboard(TMP_InputField _input)
        {
            //nonNativeKeyboard.myInputField = _input;
            NonNativeKeyboard.Instance.InputField = _input;
            NonNativeKeyboard.Instance.PresentKeyboard(_input.text);
        }
        public void ButtonCloseRegisSuccess() { OnCloseRegisSuccess(); }
        public void ButtonCloseResetPasswordSuccess() { OnCloseResetPasswordSuccess(); }
        public void ButtonCloseNotCorrectPassword() { OnCloseNotCorrectPassword(); }
        #endregion
        #region Private Fuction Action

        [SerializeField] private bool isOnline;
        [SerializeField] private bool isInspector;

        private void OnEnable()
        {
            loginPanel.SetActive(true);
        }
        private void OnStandAlone()
        {
            isOnline = false;
            isInspector = false;
            standaloneButton.image.color = new Color32(254, 50, 1, 255);
            playerButton.image.color = Color.clear;
            inspectorButton.image.color = Color.clear;
        }
        private void OnPlayer()
        {
            isOnline = true;
            isInspector = false;
            standaloneButton.image.color = Color.clear;
            playerButton.image.color = new Color32(254, 50, 1, 255);
            inspectorButton.image.color = Color.clear;
        }
        private void OnInspector()
        {
            //Set to Inpector==========================================================================??????
            isOnline = true;
            isInspector = true;
            standaloneButton.image.color = Color.clear;
            playerButton.image.color = Color.clear;
            inspectorButton.image.color = new Color32(254, 50, 1, 255);
        }
        private void OnLogin()
        {
            //Debug.Log("Login processing");
            loginButton.interactable = false;
            string getUsername = usernameInputField.text;
            string getPassword = passwordInputField.text;

            //========================================================================================================TEST==
#if UNITY_EDITOR
            getUsername = "nopparat.pgroup@gmail.com";
            getPassword = "12345";
#endif
            //============================================================================================================================

            if (string.IsNullOrEmpty(getUsername) || string.IsNullOrEmpty(getPassword))
            {
                loginButton.interactable = true;
                return;
            }

            APIManager.Instance.Login<LoginResponse>(getUsername, getPassword, (success, msg, res) =>
            {
                if (success)
                {
                    loginPanel.SetActive(false);
                    //Debug.Log("Login success");

                    nonNativeKeyboard.Close();
                    //Set to Inpector==========================================================================??????
                    if (isOnline) networkController.OnClickJoin(isInspector);
                    else networkController.StartHostLocal();
                    //Set to Inpector==========================================================================??????
                    APIManager.Instance.userEmail = getUsername;
                }
                else
                {
                    //Debug.Log(msg);
                    notCorrectPasswordPanel.SetActive(true);
                    //Debug.LogError(msg);
                }
                loginButton.interactable = true;
            });
        }
        private void OnForgotPassword()
        {
            sentEmailResetPasswordPanel.SetActive(true);
            loginPanel.SetActive(false);
        }
        private void OnSignUp()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }
        private void OnCancel()
        {
            notFoundPanel.SetActive(false);
        }
        private void OnRetry()
        {

        }
        private void OnRegister()
        {
            registerButton.interactable = false;
            string getEmail = emailInputField.text;
            string getPassword = passwordRegisInputField.text;
            string getFirstname = firstnameInputField.text;
            string getLastname = lastnameInputField.text;

            //========================================================================================================TEST==
            //getEmail = "nopparat.pgroup@gmail.com";
            //getPassword = "12345";
            //getFirstname = "nopparat";
            //getLastname = "sangpakdee";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getEmail) || string.IsNullOrEmpty(getPassword) ||
                string.IsNullOrEmpty(getFirstname) || string.IsNullOrEmpty(getLastname) || !policyToggle.isOn)
            {
                registerButton.interactable = true;
                Debug.Log("Fill All Value");
                return;
            }

            APIManager.Instance.Register<LoginResponse>(getEmail, getPassword, getFirstname, getLastname, (success, msg, res) =>
            {
                if (success)
                {
                    regisSuccessPanel.SetActive(true);
                    registerPanel.SetActive(false);
                    loginPanel.SetActive(true);
                    registerButton.interactable = true;
                    Debug.Log("Register success");
                }
                else
                {
                    registerButton.interactable = true;
                    Debug.LogError(msg);
                }
            });
        }
        private void OnBacktoLogin()
        {
            registerPanel.SetActive(false);
            sentEmailResetPasswordPanel.SetActive(false);
            resetPasswordPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        private void OnResetPassword()
        {
            string getEmail = emailResetInputField.text;
            string getToken = tokenResetInputField.text;
            string getNewPassword = newPasswordResetInputField.text;

            //========================================================================================================TEST==
            //getEmail = "nopparat.pgroup@gmail.com";
            //getNewPassword = "12345";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getEmail) || string.IsNullOrEmpty(getToken) || string.IsNullOrEmpty(getNewPassword)) return;
            
            APIManager.Instance.ResetPassword<LoginResponse>(getEmail, getToken, getNewPassword, (success, msg, res) =>
            {
                if (success)
                {
                    resetPasswordSuccessPanel.SetActive(true);
                    resetPasswordPanel.SetActive(false);
                    loginPanel.SetActive(true);
                    Debug.Log("Reset Password success");
                }
                else
                {
                    Debug.LogError(msg);
                }
            });
        }
        private void OnSentEmailResetPassword()
        {
            sendEmailPasswordButton.interactable = false;
            string getEmail = emailSentResetInputField.text;

            //========================================================================================================TEST==
            //getEmail = "nopparat.pgroup@gmail.com";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getEmail)) return;

            APIManager.Instance.SentEmailResetPasswordToken<LoginResponse>(getEmail, (success, msg, res) =>
            {
                if (success)
                {
                    sentEmailResetPasswordPanel.SetActive(false);
                    resetPasswordPanel.SetActive(true);
                    Debug.Log("Sent Token to Email success");
                }
                else
                {
                    Debug.LogError(msg);
                }
                sendEmailPasswordButton.interactable = true;
            });
        }
        private void OnCloseRegisSuccess()
        {
            regisSuccessPanel.SetActive(false);
        }
        private void OnCloseResetPasswordSuccess()
        {
            resetPasswordSuccessPanel.SetActive(false);
        }
        private void OnCloseNotCorrectPassword()
        {
            notCorrectPasswordPanel.SetActive(false);
        }
        #endregion
    }
}
