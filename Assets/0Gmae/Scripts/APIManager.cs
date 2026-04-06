using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Boy
{
    public class APIManager : MonoBehaviour
    {
        public static APIManager Instance { get; private set; }
        public string baseUrl;
        [SerializeField] private string vrstApiKey = "";
        public string userEmail = "test@gmail.com";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SaveSession<T>(string json, UnityAction<bool, string, T> callback = null)
        {
            string url = $"{baseUrl}/api/save-session";
            StartCoroutine(PostJson(url, json, callback));
        }
        public void SaveJoinCode<T>(string code, UnityAction<bool, string, T> callback = null)
        {
            if (!PlayerPrefs.HasKey("roomCode"))
            {
                Debug.LogWarning("Room code not found.");
                return;
            }

            var body = new
            {
                room_code = PlayerPrefs.GetString("roomCode"),
                key_join_multiplayer = code,
                overwrite = true,
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/save-key-join-multiplayer";
            print(json);
            StartCoroutine(PostJson(url, json, callback));
        }
        public void GetJoinCode<T>(UnityAction<bool, string, T> callback = null)
        {
            if (!PlayerPrefs.HasKey("roomCode"))
            {
                Debug.LogWarning("Room code not found.");
                return;
            }

            string url = $"{baseUrl}/api/get-key-join-multiplayer?room_code={PlayerPrefs.GetString("roomCode")}";
            print(url);
            StartCoroutine(GetRequest(url, callback));
        }
        #region PGroup
        public void Login<T>(string _email, string _password, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = _email,
                password = _password
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/login";

            StartCoroutine(PostJson(url, json, callback));
        }
        public void Register<T>(string _email, string _password,string _firstname,string _lastname, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = _email,
                password = _password,
                firstName = _firstname,
                lastName = _lastname
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/register";

            StartCoroutine(PostJson(url, json, callback));
        }
        public void SentEmailResetPasswordToken<T>(string _userEmail, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = _userEmail
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/email-reset-password";

            StartCoroutine(PostJson(url, json, callback));
        }
        public void ResetPassword<T>(string _userEmail, string _resetPasswordToken, string _newPassword, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = _userEmail,
                resetPasswordToken = _resetPasswordToken,
                newPassword = _newPassword
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/reset-password";

            StartCoroutine(PostJson(url, json, callback));
        }
        #endregion

        public IEnumerator GetRequest<T>(string url,
            UnityAction<bool, string, T> callback = null)
        {
            if (!InternetManager.Instance.InternetStatus) yield break;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                if (!string.IsNullOrEmpty(vrstApiKey))
                {
                    webRequest.SetRequestHeader("x-api-key", vrstApiKey);
                }

                yield return webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                    yield return null;
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(false, webRequest.error, default(T));
                }
                else
                {
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    T response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback?.Invoke(true, "Received", response);
                }
            }
        }

        IEnumerator PostJson<T>(string url, string jsonData,
            UnityAction<bool, string, T> callback = null)
        {
            if (!InternetManager.Instance.InternetStatus) yield break;
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonData, "application/json"))
            {
                if (!string.IsNullOrEmpty(vrstApiKey))
                {
                    webRequest.SetRequestHeader("x-api-key", vrstApiKey);
                }
                yield return webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                    yield return null;
                }

                Debug.Log(webRequest.downloadHandler.text);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(false, webRequest.error, default(T));
                }
                else
                {
                    //Debug.Log(webRequest.downloadHandler.text);
                    T res = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback?.Invoke(true, "complete", res);
                }
            }
        }
        public void DownloadImage(string url,
        Action<Texture2D> callback = null,
        Action<float> progress = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                callback?.Invoke(null);
                return;
            }

            StartCoroutine(_DownloadImage(url, callback, progress));
        }

        IEnumerator _DownloadImage(string url, Action<Texture2D> callback, Action<float> progress)
        {
            print(url);
            if (!InternetManager.Instance.InternetStatus) yield break;
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                progress?.Invoke(request.uploadProgress + request.downloadProgress);
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                callback?.Invoke(null);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                callback?.Invoke(texture);
            }
        }
    }
}

[Serializable]
public class JoinMultiplayerResponse
{
    public bool status;
    public string message;
    public string code;
    public string desc;
    public JoinMultiplayerData data;
}

[Serializable]
public class JoinMultiplayerData
{
    public string key_join_multiplayer;
    public string updatedAt;
}
[Serializable]
public class LoginResponse
{
    public bool status;
    public string message;
    public string code;
    public string desc;
}