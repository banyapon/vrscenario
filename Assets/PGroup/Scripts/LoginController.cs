using Boy;
using System;
using System.Collections;
using System.Text;
using TMPro;
using Unity.Services.Relay.Models;
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

        [Header("Button")]
        [HideInInspector][SerializeField] private Button standaloneButton;
        [HideInInspector][SerializeField] private Button connectHostButton;
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
        #endregion
        #region Public Fuction Button
        private void Awake()
        {
            standaloneButton.onClick.AddListener(() => ButtonStandalone());
            connectHostButton.onClick.AddListener(() => ButtonConnectHost());
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
        }
        public void ButtonStandalone() { OnStandAlone(); }
        public void ButtonConnectHost() { OnConnectHost(); }
        public void ButtonLogin() { OnLogin(); }
        public void ButtonForgotPassword() { OnForgotPassword(); }
        public void ButtonSignup() { OnSignUp(); }
        public void ButtonCancel() { OnCancel(); }
        public void ButtonRetry() { OnRetry(); }
        public void ButtonRegister() { OnRegister(); }
        public void ButtonBacktoLogin() { OnBacktoLogin(); }
        public void ButtonResetPassword() { OnResetPassword(); }
        public void ButtonSentEmailPassword() { OnSentEmailResetPassword(); }
        #endregion
        #region Private Fuction Action

        private bool isOnline;

        private void OnStandAlone()
        {
            isOnline = false;
            standaloneButton.image.color = new Color32(254, 50, 1, 255);
            connectHostButton.image.color = Color.clear;
        }
        private void OnConnectHost()
        {
            isOnline = true;
            standaloneButton.image.color = Color.clear;
            connectHostButton.image.color = new Color32(254, 50, 1, 255);
        }
        private void OnLogin()
        {
            string getUsername = usernameInputField.text;
            string getPassword = passwordInputField.text;

            //========================================================================================================TEST==
            getUsername = "nopparat.pgroup@gmail.com";
            getPassword = "12345";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getUsername) || string.IsNullOrEmpty(getPassword)) return;

            APIManager.Instance.Login<LoginResponse>(getUsername, getPassword, (success, msg, res) =>
            {
                if (success)
                {
                    loginPanel.SetActive(false);
                    Debug.Log("Login success");

                    if (isOnline) networkController.StartHostLocal();
                    else networkController.OnClickJoin();
                }
                else
                {
                    Debug.LogError(msg);
                }
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
            getEmail = "nopparat.pgroup@gmail.com";
            getPassword = "12345";
            getFirstname = "nopparat";
            getLastname = "sangpakdee";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getEmail) || string.IsNullOrEmpty(getPassword) ||
                string.IsNullOrEmpty(getFirstname) || string.IsNullOrEmpty(getLastname))
            {
                registerButton.interactable = true;
                Debug.Log("Fill All Value");
                return;
            }

            APIManager.Instance.Register<LoginResponse>(getEmail, getPassword, getFirstname, getLastname, (success, msg, res) =>
            {
                if (success)
                {
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
            getEmail = "nopparat.pgroup@gmail.com";
            getNewPassword = "12345";
            //============================================================================================================================

            if (string.IsNullOrEmpty(getEmail) || string.IsNullOrEmpty(getToken) || string.IsNullOrEmpty(getNewPassword)) return;
            
            APIManager.Instance.ResetPassword<LoginResponse>(getEmail, getToken, getNewPassword, (success, msg, res) =>
            {
                if (success)
                {
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
            string getEmail = emailSentResetInputField.text;

            //========================================================================================================TEST==
            getEmail = "nopparat.pgroup@gmail.com";
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
            });
        }
        #endregion
    }
}
