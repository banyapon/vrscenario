using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net;
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
        public string roomCode = "1";

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
            var body = new
            {
                room_code = roomCode,
                key_join_multiplayer = code,
                overwrite = true,
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/save-key-join-multiplayer";
            StartCoroutine(PostJson(url, json, callback));
        }
        public void GetJoinCode<T>(UnityAction<bool, string, T> callback = null)
        {
            string url = $"{baseUrl}/api/get-key-join-multiplayer?room_code={roomCode}";
            StartCoroutine(GetRequest(url, callback));
        }
        public void Login<T>(string email, string password, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = email,
                password = password
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/login";

            StartCoroutine(PostJson(url, json, callback));
        }
        public void Register<T>(string email, string password,string firstname,string lastname, UnityAction<bool, string, T> callback = null)
        {
            var body = new
            {
                userEmail = email,
                password = password,
                firstName = firstname,
                lastName = lastname
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/register";

            StartCoroutine(PostJson(url, json, callback));
        }

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

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(false, webRequest.error, default(T));
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    T res = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback?.Invoke(true, "complete", res);
                }
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
[Serializable]
public class RegisterResponse
{
    public bool status;
    public string message;
    public string code;
    public string desc;
}