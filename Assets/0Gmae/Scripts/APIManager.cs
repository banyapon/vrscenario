using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;

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
            var body = new
            {
                room_code = "1",
                key_join_multiplayer = code,
                overwrite = true,
            };

            string json = JsonConvert.SerializeObject(body);
            string url = $"{baseUrl}/api/save-key-join-multiplayer";
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
public class Response
{
    public string status;
    public string message;
}

[Serializable]
public class Response<T>
{
    public string status;
    public string message;
    public T data;
}