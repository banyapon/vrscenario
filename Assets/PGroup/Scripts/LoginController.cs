using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PGroup
{
    public class LoginController : MonoBehaviour
    {
        [Header("Panel")]
        [HideInInspector][SerializeField] private GameObject loginPanel;
        [HideInInspector][SerializeField] private GameObject registerPanel;
        [HideInInspector][SerializeField] private GameObject resetPasswordPanel;
        [HideInInspector][SerializeField] private GameObject searchingPanel;
        [HideInInspector][SerializeField] private GameObject notFoundPanel;

        [Header("Button")]
        [HideInInspector][SerializeField] private Button standaloneButton;
        [HideInInspector][SerializeField] private Button connectHostButton;
        [HideInInspector][SerializeField] private Button forgotPasswordButton;
        [HideInInspector][SerializeField] private Button loginButton;
        [HideInInspector][SerializeField] private Button signUpButton;
        [HideInInspector][SerializeField] private Button retryButton;
        [HideInInspector][SerializeField] private Button cancelButton;
        [HideInInspector][SerializeField] private Button registerButton;
        [HideInInspector][SerializeField] private Button backToLoginFromRegisButton;
        [HideInInspector][SerializeField] private Button resetPasswordButton;
        [HideInInspector][SerializeField] private Button backToLoginFromResetButton;
            
        [Header("InputField")]
        [HideInInspector][SerializeField] private TMP_InputField usernameInputField;
        [HideInInspector][SerializeField] private TMP_InputField passwordInputField;
        [HideInInspector][SerializeField] private TMP_InputField firstnameInputField;
        [HideInInspector][SerializeField] private TMP_InputField lastnameInputField;
        [HideInInspector][SerializeField] private TMP_InputField emailInputField;
        [HideInInspector][SerializeField] private TMP_InputField passwordRegisInputField;
        [HideInInspector][SerializeField] private TMP_InputField confirmPasswordInputField;
        [HideInInspector][SerializeField] private TMP_InputField emailResetInputField;

        [Header("Toggle")]
        [HideInInspector][SerializeField] private Toggle policyToggle;

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
        #endregion

        #region Private Fuction Action
        private void OnStandAlone()
        {
            standaloneButton.image.color = new Color32(254, 50, 1, 255);
            connectHostButton.image.color = Color.clear;
        }
        private void OnConnectHost()
        {
            standaloneButton.image.color = Color.clear;
            connectHostButton.image.color = new Color32(254, 50, 1, 255);
        }
        private void OnLogin()
        {

        }
        private void OnForgotPassword()
        {

        }
        private void OnSignUp()
        {

        }
        private void OnCancel()
        {

        }
        private void OnRetry()
        {

        }
        private void OnRegister()
        {

        }
        private void OnBacktoLogin()
        {

        }
        private void OnResetPassword()
        {

        }
        #endregion
    }
}
